using System.Reflection;
using System.Linq;

// var baseSoapType = typeof(System.Web.Services.Protocols.SoapHttpClientProtocol);
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

public class CsxTest {
    public Action TestAction { get; set; }
    public string Title { get; set; }
    public CsxTest() {
    }
}

var testsRun = 0;
var testsSucceeded = 0;
var testsFailed = 0;
var tests = new List<CsxTest>();

Action<string, Action> it = (testTitle, testAction) => {
    tests.Add(new CsxTest {
        TestAction = testAction,
	Title = testTitle
    });
};

Action runTests = () => {
    Console.WriteLine("Running tests ...");
    foreach(var test in tests) {
        try {
	    testsRun++;
	    test.TestAction();
	    Console.ForegroundColor = System.ConsoleColor.Green;
	    Console.WriteLine(test.Title + " SUCCESS.");
	    testsSucceeded++;
	} catch (Exception ex) {
	    testsFailed++;
	    Console.ForegroundColor = System.ConsoleColor.Red;
	    Console.WriteLine(test.Title + " FAILED.");
	    Console.WriteLine(ex.ToString());
	}
    }
    Console.ForegroundColor = System.ConsoleColor.Gray;
    Console.WriteLine("");
    Console.WriteLine("Tests run: " + testsRun);
    Console.WriteLine("Tests succeeded: " + testsSucceeded);
    Console.WriteLine("Tests failed: " + testsFailed);
    if(testsRun == testsSucceeded) {
        Console.ForegroundColor = System.ConsoleColor.Green;
        Console.WriteLine("All tests succeeded.");
	Console.ForegroundColor = System.ConsoleColor.Gray;
    } else {
        Console.ForegroundColor = System.ConsoleColor.Red;
	Console.WriteLine("Some test(s) failed.");
	Console.ForegroundColor = System.ConsoleColor.Gray;
    }
    Console.WriteLine("");
};
