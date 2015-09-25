
#r "System.Web"
#r "System.Web.Services"

using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.IO;

//Create the WSDL-generated class
var wsdlProgram = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\wsdl.exe";
var wsdlUrl = Env.ScriptArgs[0];

Console.WriteLine("WSDL found?" + File.Exists(wsdlProgram));
//var wsdlOutputBinary = "ServiceClient.dll";

var wsdlOutputBinary = Env.ScriptArgs[0].Substring(Env.ScriptArgs[0].LastIndexOf("/") + 1,
  Env.ScriptArgs[0].Length - Env.ScriptArgs[0].LastIndexOf(".svc") + 1) + ".dll";
var wsdlOutputFile = Path.GetTempFileName();

var wsdlProcess = new Process();
wsdlProcess.StartInfo = new ProcessStartInfo(wsdlProgram, wsdlUrl + " /o:" + wsdlOutputFile);
wsdlProcess.Start();
wsdlProcess.WaitForExit();

Console.WriteLine("Temporary output file?" + wsdlOutputFile + " exists?" + File.Exists(wsdlOutputFile));

//TODO: probably not an issue with these small client files, but if it got too big, this would be an issue
var entireSvcFile = File.ReadAllText(wsdlOutputFile);
File.WriteAllText(wsdlOutputFile, "namespace ScaffoldedClient {" + entireSvcFile + "}");

var cscProcess = new Process();

var cscProgram = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

Console.WriteLine("csc found?" + File.Exists(cscProgram));

var cscArguments = @"/t:library /r:System.Web.Services.dll /r:System.Xml.dll /out:" + wsdlOutputBinary + " " + wsdlOutputFile;
cscProcess.StartInfo = new ProcessStartInfo(cscProgram, cscArguments);
cscProcess.Start();
cscProcess.WaitForExit();

Console.WriteLine("Does " + wsdlOutputBinary + " exist?" + File.Exists(wsdlOutputBinary));
Console.WriteLine("Looking for " + Path.GetFullPath(wsdlOutputBinary));

Assembly assembly;
AssemblyName an;
Type baseSoapType;

try {
an = AssemblyName.GetAssemblyName(wsdlOutputBinary);
//assembly = Assembly.ReflectionOnlyLoadFrom(Path.GetFullPath(wsdlOutputBinary));
assembly = Assembly.LoadFrom(Path.GetFullPath(wsdlOutputBinary));
baseSoapType = typeof(System.Web.Services.Protocols.SoapHttpClientProtocol);

foreach(var t in assembly.GetTypes()) { Console.WriteLine(t.Name); };
} catch(Exception ex) {
  Console.WriteLine("Exception loading types...");
}

var types = assembly.GetTypes()
.Where(t => t != baseSoapType && baseSoapType.IsAssignableFrom(t));

var scriptTemplate = @"
#" + @"r ""System""
#" + @"r ""System.Core""
#" + @"r ""System.Web""
#" + @"r ""System.Web.Services""
#" + @"r ""System.Xml.Linq""
#" + @"r ""System.Data.DataSetExtensions""
#" + @"r ""Microsoft.CSharp""
#" + @"r ""System.Data""
#" + @"r ""System.Net.Http""
#" + @"r ""System.Xml""

#" + @"r ""{0}""

#" + @"load ""Assert.cs""
#" + @"load ""TestHelper.cs""

var client = new {1};

";


var scaffoldFileBuilder = new StringBuilder();

Console.WriteLine("Enumerating types...");

foreach(var clientType in types) {
  var fileName = "Test" + clientType.Name + ".csx";
  Console.WriteLine("Processing " + clientType.Name + " as " + fileName);
  var references = String.Format(scriptTemplate,
   wsdlOutputBinary,
   clientType.FullName);

   Console.WriteLine("Appending references...");

   scaffoldFileBuilder.Append(references);

   var singleTest = @"
//it(""{0}"",
//() => {{
  //{1}
  //var resp = client.{2}({3});
//}});";

//TODO: more precise detection of async method exclusion
//client.GetType().GetMembers(System.Reflection.BindingFlags.DeclaredOnly).Where(m => m.MemberType == System.Reflection.MemberTypes.Method)
//  var methods = clientType.GetMethods()
  //Console.WriteLine("DeclaredOnly Members " + clientType.GetMembers(BindingFlags.DeclaredOnly).Length);
  //var methodNames = clientType.GetMembers(BindingFlags.DeclaredOnly)
  var methodNames = clientType.GetMembers()
  .Where(m => m.DeclaringType == clientType
  && m.MemberType == MemberTypes.Method
  && !m.Name.StartsWith("Begin")
  && !m.Name.EndsWith("Async")
  && !m.Name.StartsWith("End")
  && !m.Name.StartsWith("add_")
  && !m.Name.StartsWith("remove_"))
  .Select(m => m.Name);
  Console.WriteLine("Found " + methodNames.Count());
  var methods = clientType.GetMethods().Where(m => methodNames.Contains(m.Name));
  foreach(var method in methods) {

    var methodParams = String.Join(", ", method.GetParameters()
    .Select(p => p.ParameterType.Name + " " + p.Name));

    scaffoldFileBuilder.Append(String.Format(singleTest,
    "performs " + method.Name,
    method.Name + " " + methodParams,
    method.Name,
    ""));
  }

scaffoldFileBuilder.Append("\r\nrunTests();");

  Console.WriteLine("Writing scaffolding to " + fileName);
  File.WriteAllText(fileName, scaffoldFileBuilder.ToString());
}

//
// public class ClientFactory {
//   public static T CreateClient
// }
//
// Func<object> createClient = () => {
//    var soapClient = AppDomain.CurrentDomain.GetAssemblies()
//   .SelectMany(a => a.GetTypes())
//   .First(t => t != baseSoapType && baseSoapType.IsAssignableFrom(t));
//   return Activator.CreateInstance(soapClient);
// };

//Write a CSX script that references the given library
