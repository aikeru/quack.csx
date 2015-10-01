#r "System.Configuration"

//Expose a simple API for adding and removing configuration

public static class QuackConfig {
    public static void SetConnectionString(
            string name,
            string connString,
            string providerName) {
        //Loop through the connection strings
        //If we find a matching string, override it, otherwise add it
        System.Configuration.ConnectionStringSettings connSettings = null;
        foreach(var currentConnSettings in System.Configuration.ConfigurationManager
                .ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>()) {
            if(((System.Configuration.ConnectionStringSettings)currentConnSettings).Name == name) {
               connSettings = currentConnSettings;
               break;
            }
        }

        if(connSettings != null) {
            updateConnectionString(connSettings, name, connString, providerName);
        } else {
            createConnectionString(name, connString, providerName);
        }

    }

    private static void updateConnectionString(System.Configuration.ConnectionStringSettings connSettings,
            string name,
            string connString,
            string providerName) {
        
        var readOnlyField = typeof(System.Configuration.ConfigurationElement)
            .GetField("_bReadOnly",
                    System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.NonPublic);
        readOnlyField.SetValue(connSettings, false);
        connSettings.ConnectionString = connString;
        connSettings.ProviderName = providerName;
        readOnlyField.SetValue(connSettings, true);
    }

    private static void createConnectionString(string name, string connString, string providerName) {
        var readOnlyField = typeof(System.Configuration.ConfigurationElementCollection)
            .GetField("bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        readOnlyField.SetValue(System.Configuration.ConfigurationManager.ConnectionStrings, false);

        System.Configuration.ConfigurationManager
            .ConnectionStrings
            .Add(new System.Configuration.ConnectionStringSettings(name, connString, providerName));

        readOnlyField.SetValue(System.Configuration.ConfigurationManager.ConnectionStrings, true);
    }
}
