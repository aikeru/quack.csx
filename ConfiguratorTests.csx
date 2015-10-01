#load ".\RapidTesting\Assert.cs"
#load ".\RapidTesting\TestHelper.cs"

#load "Configurator.cs"

it("should be able to create a new connection string", () => {
   QuackConfig.SetConnectionString("Test", "Server=SqlDatabase1", "Sql.Data.Client"); 
   Assert.IsTrue(System.Configuration.ConfigurationManager.ConnectionStrings["Test"].ConnectionString == "Server=SqlDatabase1");
});

it("should be able to overwrite an existing connection string", () => {
    Assert.IsTrue(System.Configuration.ConfigurationManager.ConnectionStrings["Test"].ConnectionString == "Server=SqlDatabase1");
    QuackConfig.SetConnectionString("Test", "Server=SqlDatabase2", "Sql.Data.Client");
    Assert.IsTrue(System.Configuration.ConfigurationManager.ConnectionStrings["Test"].ConnectionString == "Server=SqlDatabase2");
});

it("should not leave connection string collection as editable", () => {
    var success = true;
    try {
        System.Configuration.ConfigurationManager
            .ConnectionStrings
            .Add(new System.Configuration.ConnectionStringSettings("Bob", "String", "Provider"));
        success = false;
    } catch(Exception ex) {
        //Success
    }
    if(!success) {
        throw new Exception("Connection string collection should not be editable!");
    }
});

it("should not leave connection string as editable", () => {
    var success = true;
    try {
        System.Configuration.ConfigurationManager
            .ConnectionStrings["Test"].ConnectionString = "Dummy";
        success = false;
    } catch(Exception ex) {
        //Success
    }
    if(!success) { throw new Exception("Connection string collection should not be editable!"); }
});

runTests();
