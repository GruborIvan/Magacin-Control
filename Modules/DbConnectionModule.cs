using Microsoft.Extensions.Configuration;
using System.IO;

namespace CSS_MagacinControl_App.Modules
{
    public class DbConnectionModule
    {
        public static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json")
                 .Build();

            return configuration.GetConnectionString("DeployConnectionString");
        }
    }
}