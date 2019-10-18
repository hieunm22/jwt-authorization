using System.IO;
using Microsoft.Extensions.Configuration;

namespace FormHTML.Common
{
    public class AppSettings
    {
        private static IConfigurationRoot instance = null;

        // This object holds configuration from both the appsettings.json config file and any Lambda environment
        // variables that were set.  It's a singleton accessed via a static property so it will be created once and will
        // be accessible from anywhere in your code after that.  I recommend only accessing it in a static context (e.g. your
        // static bootstrapping methods) and injecting configuration properties as strings/etc directly into your services to keep
        // them unit testable.
        //
        // IMPORTANT: the ConfigurationBuilder class merges env variable & appsettings config into one object, so make sure none of the keys
        // in your appsettings.json object are named the same as one of your environment variables, since one will overwrite the other.
        public static IConfigurationRoot Instance
        {
            get
            {
                return instance ?? (instance = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile($"appsettings.json", optional: false)
                           //.AddJsonFile($"appsettings.{environmentName}.json", optional: false)
                           .Build());
            }
        }
    }
}