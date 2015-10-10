#load "Quack.Selenium.cs"

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

using OpenQA.Selenium.Interactions;

Func<IWebDriver> setup = () => {
  IWebDriver driver = new FirefoxDriver();
  driver.Navigate().GoToUrl("http://www.google.com");
  ((IJavaScriptExecutor)driver).ExecuteScript(System.IO.File.ReadAllText(".\\Selenium\\jquery2.min.js"));
  return driver;
};

it("uses setup and finds the cheese", () => {
  var driver = setup();
  new QuackjQuery("[name='q']", driver).First().SendKeys("Cheese");
  new QuackjQuery("[name='q']", driver).First().Submit();
  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
  wait.Until((d) => { return d.Title.ToLower().StartsWith("cheese"); });
  Assert.IsTrue(driver.Title.Trim() == "Cheese - Google Search");
  driver.Quit();
});

it("uses setup and hovers the button", () => {
  var driver = setup();
  var qe = new QuackjQuery("[value=\"I'm Feeling Lucky\"]", driver);
  Actions builder = new Actions(driver);
  Actions hoverClick = builder.MoveToElement(qe.First());
  hoverClick.Build().Perform();
  Assert.IsTrue(qe.Css("visibility") == "visible");
  driver.Quit();
});

runTests();

//IWebDriver driver = new FirefoxDriver();

//driver.Navigate().GoToUrl("http://www.google.com");
//IWebElement query = driver.FindElement(By.Name("q"));
//query.SendKeys("Cheese");

//query.Submit();
// Should see: "Cheese - Google Search"
//System.Console.WriteLine("Page title is: " + driver.Title);

//Close the browser
//driver.Quit();

//Console.WriteLine("WebDriver loaded!");
