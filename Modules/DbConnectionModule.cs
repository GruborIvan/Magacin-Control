using Microsoft.Extensions.Configuration;
using System;

namespace CSS_MagacinControl_App.Modules
{
    public class DbConnectionModule
    {
        public static string GetConnectionString()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                 .Build();

        #if DEBUG
            return configuration.GetConnectionString("LocalConnectionString");
        #else
            return configuration.GetConnectionString("DeployConnectionString");
        #endif
        }
    }
}