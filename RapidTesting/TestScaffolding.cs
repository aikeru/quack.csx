
#r "System.Web"
#r "System.Web.Services"
#r "System.Runtime.Serialization"

using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.IO;

//Create the WSDL-generated class
//var wsdlProgram = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\wsdl.exe";
var svcUtilProgram = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\svcutil.exe"; //"%1" /out:output.cs

var svcUrl = Env.ScriptArgs[0];
var rewrite = Env.ScriptArgs.Count() > 1 ? Env.ScriptArgs[1] : "";

Console.WriteLine("SvcUtil found?" + File.Exists(svcUtilProgram));
var outputBinary = "ServiceClient.dll";

var svcUtilOutputBinary = Env.ScriptArgs[0].Substring(Env.ScriptArgs[0].LastIndexOf("/") + 1,
  Env.ScriptArgs[0].Length - Env.ScriptArgs[0].LastIndexOf(".svc") + 1) + ".dll";
var svcUtilOutputFile = Path.GetTempFileName();

var svcUtilProcess = new Process();
Console.WriteLine("exe: " + svcUtilProgram + " " + svcUrl + " /out: " + svcUtilOutputFile);

svcUtilOutputFile = svcUtilOutputFile + ".cs"; //it does this ....

svcUtilProcess.StartInfo = new ProcessStartInfo(svcUtilProgram, svcUrl + " /out:" + svcUtilOutputFile);
svcUtilProcess.Start();
svcUtilProcess.WaitForExit();

Console.WriteLine("Temporary output file?" + svcUtilOutputFile + " exists?" + File.Exists(svcUtilOutputFile));

//TODO: probably not an issue with these small client files, but if it got too big, this would be an issue
var entireSvcFileLines = File.ReadAllText(svcUtilOutputFile).Split(new [] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
StringBuilder sbFinalCs = new StringBuilder();
var hasPassedAssembly = false;
var clientNamespaces = new List<string>();
var hasFoundClientNamespace = false;
foreach(var line in entireSvcFileLines) {
	if(line.Contains("namespace")) {
		clientNamespaces.Add(line.Substring("namespace ".Length));
	}
	if(!hasPassedAssembly && line.Contains("assembly")) {
		hasPassedAssembly = true;
		sbFinalCs.AppendLine(line);
		sbFinalCs.AppendLine("namespace ScaffoldedClient {");
	} else {
		sbFinalCs.AppendLine(line);
	}
}
sbFinalCs.AppendLine("}");
//File.WriteAllText(svcUtilOutputFile, "namespace ScaffoldedClient {" + entireSvcFile + "}");
File.WriteAllText(svcUtilOutputFile, sbFinalCs.ToString());

var cscProcess = new Process();

var cscProgram = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe";

Console.WriteLine("csc found?" + File.Exists(cscProgram));

var cscArguments = @"/t:library /r:System.Web.Services.dll /r:System.Xml.dll /out:" + outputBinary + " " + svcUtilOutputFile;
Console.WriteLine("exe: " + cscProgram + " " + cscArguments);
cscProcess.StartInfo = new ProcessStartInfo(cscProgram, cscArguments);
cscProcess.Start();
cscProcess.WaitForExit();

Console.WriteLine("Does " + outputBinary + " exist?" + File.Exists(outputBinary));
Console.WriteLine("Looking for " + Path.GetFullPath(outputBinary));

Assembly assembly;
AssemblyName an;
Type baseType;

try {
an = AssemblyName.GetAssemblyName(outputBinary);
//assembly = Assembly.ReflectionOnlyLoadFrom(Path.GetFullPath(wsdlOutputBinary));
assembly = Assembly.LoadFrom(Path.GetFullPath(outputBinary));
baseType = typeof(System.Runtime.Serialization.IExtensibleDataObject);

foreach(var t in assembly.GetTypes()) { Console.WriteLine(t.Name); };
} catch(Exception ex) {
  Console.WriteLine("Exception loading types...");
}

//var types = assembly.GetTypes()
//.Where(t => t != baseType && baseType.IsAssignableFrom(t));
var types = assembly.GetTypes()
	.Where(t => t.Name.EndsWith("Client"));

var bareSvcUrl = svcUrl.Replace("?wsdl", "");
	
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
#" + @"r ""System.Runtime.Serialization.dll""
#" + @"r ""System.ServiceModel""

#" + @"r ""{0}""

#" + @"load ""Assert.cs""
#" + @"load ""TestHelper.cs""

//Client namespaces
{2}

System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding();
var endpointAddress = new System.ServiceModel
	.EndpointAddress(""{3}"");

var client = new {1}(binding, endpointAddress);

";


var scaffoldFileBuilder = new StringBuilder();

Console.WriteLine("Enumerating types..." + types.Count());

foreach(var clientType in types) {
  var fileName = "Test" + clientType.Name + ".csx";
  Console.WriteLine("Processing " + clientType.Name + " as " + fileName);
  var references = String.Format(scriptTemplate,
   outputBinary,
   clientType.FullName,
   clientNamespaces.Select(cn => "using ScaffoldedClient." + cn + ";").Aggregate((tot, cur) => tot + "\r\n" + cur),
   bareSvcUrl);

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
  if(File.Exists(fileName) && rewrite.StartsWith("r")) {
	File.WriteAllText(fileName, scaffoldFileBuilder.ToString());
  }
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
