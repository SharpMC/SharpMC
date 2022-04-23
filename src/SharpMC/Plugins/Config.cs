using System;
using System.IO;
using System.Text;
using SharpMC.Admin;
using static SharpMC.Settings.ConfigManager;

namespace SharpMC.Plugins
{
    public static class Config
    {
        public static ServerSettings Server { get; } = GetConfig<ServerSettings>();

        public static CustomConfig Custom { get; } = new CustomConfig();

        public class CustomConfig
        {
            public T GetProperty<T>(string key, T defaultValue)
            {
                var value = GetRawValue(key, defaultValue);
                return value;
            }
        }

        public static void Check()
        {
            var serverRoot = Environment.CurrentDirectory;
            var serverFile = Path.Combine(serverRoot, ConfigFile);
            if (File.Exists(serverFile))
                return;
            var text = string.Empty;
            File.WriteAllText(serverFile, text, Encoding.UTF8);
        }
    }
}