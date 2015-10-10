#r ".\scriptcs_packages\Selenium.WebDriver.2.48.0\lib\net40\WebDriver.dll"
#r ".\scriptcs_packages\Selenium.Support.2.48.0\lib\net40\WebDriver.Support.dll"

#load "..\RapidTesting\Assert.cs"
#load "..\RapidTesting\TestHelper.cs"

using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

public class QuackSelenium {
  public static void WaitForJavaScript(string jsExp, IWebDriver driver) {
    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    wait.Until((d) => {
      var didHappen = (bool)((IJavaScriptExecutor)driver).ExecuteScript(jsExp);
      return didHappen;
    });
  }
  public static void WaitForAjax(IWebDriver driver) {
      QuackSelenium.WaitForJavaScript("return $.active === 0;", driver);
  }
}

public class QuackjQuery {
  private string _selector = "";
  public IWebDriver Driver = null;

  public QuackjQuery(string selector,
   IWebDriver driver) {
    _selector = selector.Replace("'", "\\'");
    Driver = driver;
  }
  public IWebElement First() {
    var script = "return $('" + _selector + "')[0]";
    return (IWebElement) ((IJavaScriptExecutor)Driver).ExecuteScript(script);
  }
  public void Click() {
    First().Click();
  }
  public string Css(string cssName) {
    var script = "return $('" + _selector + "').css('" + cssName + "')";
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript(script);
  }
  public string Attr(string attName) {
    var script = "return $('" + _selector + "').attr('" + attName + "')";
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript(script);
  }
  public string Attr(string attName, object attValue) {
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript("return $('" + _selector + "').attr('" + attName + "', " + attValue.ToString() + ")");
  }
  public string Prop(string propName) {
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript("return $('" + _selector + "').prop('" + propName + "')");
  }
  public string Prop(string propName, object propValue) {
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript("return $('" + _selector + "').prop(" + propName + "," + propValue.ToString() + ")");
  }
  public string Value() {
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript("return $('" + _selector + "').val()");
  }
  public string Value(object nVal) {
    return (string)((IJavaScriptExecutor)Driver).ExecuteScript("return $('" + _selector + "').val(" + nVal.ToString() + ")");
  }
  /*public bool Exists() {
    return (bool)((JavascriptExecutor)Driver).ExecuteScript(@"
      try {
        return $('').length > 0;
      } catch {
        return false;
      }
    ");
  }*/
}
