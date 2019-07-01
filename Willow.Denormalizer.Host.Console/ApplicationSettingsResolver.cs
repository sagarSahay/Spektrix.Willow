namespace Willow.Denormalizer.Host.Console
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class ApplicationSettingsResolver
    {
        private const string DefaultAppsettingsJsonFile = "appsettings";
        
        public static Func<string, string> GetSettingsResolver(
            string appsettingsBasePath = null,
            string appsettingsFile = "appsettings",
            string environmentVariablesPrefix = "SPX_",
            string[] args = null)
        {
            var configurationBuilder = new ConfigurationBuilder();
            if (!string.IsNullOrWhiteSpace(appsettingsBasePath))
                configurationBuilder.SetBasePath(appsettingsBasePath);
            if (!string.IsNullOrWhiteSpace(appsettingsFile))
            {
                configurationBuilder.AddJsonFile(string.Format("{0}.json", (object) appsettingsFile));
                string environmentVariable = Environment.GetEnvironmentVariable("SPX_ENVIRONMENT");
                if (!string.IsNullOrWhiteSpace(environmentVariable))
                    configurationBuilder.AddJsonFile(string.Format("{0}.{1}.json",
                            (object) appsettingsFile,
                            (object) environmentVariable),
                        true);
            }
            if (!string.IsNullOrEmpty(environmentVariablesPrefix))
                configurationBuilder.AddEnvironmentVariables(environmentVariablesPrefix);
            if (args != null)
                configurationBuilder.AddCommandLine(args);
            IConfigurationRoot configuration = configurationBuilder.Build();
            return (name) =>
            {
                string configValue = configuration.GetSection(name).Value;
                return configValue;
            };
        }
    }
}