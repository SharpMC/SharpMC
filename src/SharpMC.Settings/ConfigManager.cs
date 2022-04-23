using System;
using Microsoft.Extensions.Configuration;

namespace SharpMC.Settings
{
    public static class ConfigManager
    {
        public const string ConfigFile = "server.ini";

        private static readonly IConfiguration Configuration;

        static ConfigManager()
        {
            var args = Environment.GetCommandLineArgs();
            Configuration = new ConfigurationBuilder()
                .AddIniFile(ConfigFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }

        public static T GetConfig<T>(string key = null) where T : new()
        {
            var instance = new T();
            if (string.IsNullOrWhiteSpace(key))
                Configuration.Bind(instance);
            else
                Configuration.Bind(key, instance);
            return instance;
        }

        public static T GetRawValue<T>(string key, T defaultValue)
        {
            var value = Configuration.GetValue(key, defaultValue);
            return value;
        }
    }
}