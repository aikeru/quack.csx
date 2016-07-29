#load "JSONObject.cs"

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
public class QuackResult {
  public bool Success { get; set; }
  public string ErrorMessage { get; set; }
}

var results = new List<QuackResult>();


public class Quack {
  public static bool OutputJson { get; set; }
  public static void Log(string output) {
    if(!OutputJson) {
      Console.WriteLine(output);
    }
  }
}

if(Env.ScriptArgs.Count() > 0) {
  if(Env.ScriptArgs[0] == "/json") { Quack.OutputJson = true; }
} else {
  Quack.OutputJson = false;
}

var testsRun = 0;
var testsSucceeded = 0;
var testsFailed = 0;
var tests = new List<CsxTest>();
CsxTest singleTest = null;

Action<string, Action> it = (testTitle, testAction) => {
    tests.Add(new CsxTest {
        TestAction = testAction,
	Title = testTitle
    });
};

Action<string, Action> itOnly = (testTitle, testAction) => {
	singleTest = new CsxTest {
		TestAction = testAction,
		Title = testTitle
	};
};

Action runTests = () => {
    Quack.Log("Running tests ...");
	if(singleTest != null) {
		try {
			testsRun++;
			singleTest.TestAction();
			Console.ForegroundColor = System.ConsoleColor.Green;
			Quack.Log(singleTest.Title + " SUCCESS.");
			results.Add(new QuackResult {
			  Success = true,
			  ErrorMessage = ""
			});
			testsSucceeded++;
		} catch (Exception ex) {
			testsFailed++;
			Console.ForegroundColor = System.ConsoleColor.Red;
			Quack.Log(singleTest.Title + " FAILED.");
			Quack.Log(ex.ToString());
			results.Add(new QuackResult {
			  Success = false,
			  ErrorMessage = ex.ToString()
			});
		}
	} else {
		foreach(var test in tests) {
			try {
			testsRun++;
			test.TestAction();
			Console.ForegroundColor = System.ConsoleColor.Green;
			Quack.Log(test.Title + " SUCCESS.");
		  results.Add(new QuackResult {
			  Success = true,
			  ErrorMessage = ""
			});
			testsSucceeded++;
			} catch (Exception ex) {
				testsFailed++;
				Console.ForegroundColor = System.ConsoleColor.Red;
				Quack.Log(test.Title + " FAILED.");
				Quack.Log(ex.ToString());
			  results.Add(new QuackResult {
				  Success = false,
				  ErrorMessage = ex.ToString()
				});
			}
		}
	}
    Console.ForegroundColor = System.ConsoleColor.Gray;
    Quack.Log("");
    Quack.Log("Tests run: " + testsRun);
    Quack.Log("Tests succeeded: " + testsSucceeded);
    Quack.Log("Tests failed: " + testsFailed);
    if(testsRun == testsSucceeded) {
        Console.ForegroundColor = System.ConsoleColor.Green;
        Quack.Log("All tests succeeded.");
	Console.ForegroundColor = System.ConsoleColor.Gray;
    } else {
        Console.ForegroundColor = System.ConsoleColor.Red;
		Quack.Log("Some test(s) failed.");
		Console.ForegroundColor = System.ConsoleColor.Gray;
    }
    Quack.Log("");

    if(Quack.OutputJson) {
      Console.Write(new JSONObject(new {
        testsRun = testsRun,
        testsSucceeded = testsSucceeded,
        testsFailed = testsFailed,
        testData = results.Select((r) => new JSONObject(r)).ToArray()
      }).Render());
    }
};
