# quack.csx
A series of ScriptCS scripts to write quick C# tests.<br/>
Possibly at some point will add some scripts to help decouple things when I want to work on small parts of a (possibly sub-optimally architected) large application.<br/>
Maybe at some point will hook this up with a gulp watch process to run tests continuously (like ncrunch).

### Why?

I want a lightweight, modular testing framework that doesn't add to my solution file or Visual Studio's large process.<br/>
I also want to rapidly iterate over integration/functional/behavior tests as a way to prove out my work as I code it.

### Mini-Testing Framework

Behaves like some of the JavaScript BDD test frameworks. Meant to be run from ```scriptcs```.

```C#
//Import assert and helper
#load "Assert.cs"
#load "TestHelper.cs"

//Write tests
it("should illustrate how to use a framework",
() => {
  var isUnderstood = User.Understands;
  Assert.IsTrue(isUnderstood);
});

//Run tests at the end of the file
runTests();
```

### WCF Tests

1. Run ```scriptcs TestScaffolding.cs -- http://mywcfendpoint/MyService.svc?wsdl```
2. Modify the scaffolding generated in ```TestMyService.csx``` file
3. Run tests via ```scriptcs```

### Quack?
Yup.
