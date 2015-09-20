#load "Assert.cs"
#load "TestHelper.cs"

it("should illustrate how tests are written", () => {
  var testRan = true;
  Assert.IsTrue(testRan);
});

it("should illustrate a failing test", () => {
  throw new Exception("Test explode!");
});

runTests();
