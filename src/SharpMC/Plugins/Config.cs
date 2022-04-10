using System;
using System.IO;
using SharpMC.API.Enums;

namespace SharpMC.Plugins
{
    public static class Config
    {
        public static string ConfigFile = string.Empty;
        public static string[] InitialValue;
        
        private static string _fileContents = string.Empty;
        private static bool _configChanged = false;

        public static bool Check()
        {
            if (!File.Exists(ConfigFile))
            {
                File.WriteAllLines(ConfigFile, InitialValue);
                return Check();
            }
            _fileContents = File.ReadAllText(ConfigFile);
            return true;
        }

        private static GameMode ReadGamemode(string rule, GameMode @default)
        {
            try
            {
                var gm = ReadString(rule);
                switch (gm)
                {
                    case "1":
                    case "creative":
                        return GameMode.Creative;
                    case "0":
                    case "survival":
                        return GameMode.Survival;
                    case "2":
                    case "adventure":
                        return GameMode.Adventure;
                    default:
                        return @default;
                }
            }
            catch
            {
                return @default;
            }
        }

        public static GameMode GetProperty(string property, GameMode @default)
        {
            return ReadGamemode(property, @default);
        }

        public static bool GetProperty(string property, bool @default)
        {
            return ReadBoolean(property, @default);
        }

        public static int GetProperty(string property, int @default)
        {
            return ReadInt(property, @default);
        }

        public static string GetProperty(string property, string @default)
        {
            try
            {
                return ReadString(property);
            }
            catch
            {
                return @default;
            }
        }

        private static int ReadInt(string rule, int @default)
        {
            try
            {
                return Convert.ToInt32(ReadString(rule));
            }
            catch
            {
                return @default;
            }
        }

        private static bool ReadBoolean(string rule, bool @default)
        {
            try
            {
                var d = ReadString(rule);
                return Convert.ToBoolean(d);
            }
            catch
            {
                return @default;
            }
        }

        private static string ReadString(string rule)
        {
            var t = new[] {"\r\n", "\n", Environment.NewLine};
            var lines = _fileContents.Split(t, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.ToLower().StartsWith(rule.ToLower() + "="))
                {
                    var value = line.Split('=')[1];
                    if (rule.ToLower() == "motd")
                        return value;
                    return value.ToLower();
                }
            }
            throw new EntryPointNotFoundException("The specified property was not found.");
        }

        public static void SetProperty(string property, string value)
        {
            var lined = "";
            var t = new[] {"\r\n", "\n", Environment.NewLine};
            var lines = _fileContents.Split(t, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (line.ToLower().StartsWith(property.ToLower() + "="))
                {
                    lined = line;
                    // Correct line is found
                    break;
                }
            }
            if (lined != "")
            {
                _fileContents = _fileContents.Replace(lined,
                    $"{lined.Split('=')[0]}={value}");
            }
            else
            {
                _fileContents += $"{property}={value}\r\n";
            }
            _configChanged = true;
        }

        public static void SaveConfig()
        {
            if (_configChanged)
            {
                File.WriteAllText(ConfigFile, _fileContents);
            }
        }
    }
}