using System;
using System.IO;
using SharpMC.Enums;

namespace SharpMC.Core.Utils
{
	public class Config
	{
		public static string ConfigFile = string.Empty;
		private static string _fileContents = string.Empty;
		public static string[] InitialValue;
		private static bool ConfigChanged = false;

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

		public static Gamemode GetProperty(string property, Gamemode defaultValue)
		{
			return ReadGamemode(property, defaultValue);
		}

		public static bool GetProperty(string property, bool defaultValue)
		{
			return ReadBoolean(property, defaultValue);
		}

		public static int GetProperty(string property, int defaultValue)
		{
			return ReadInt(property, defaultValue);
		}

		public static string GetProperty(string property, string defaultValue)
		{
			try
			{
				return ReadString(property);
			}
			catch
			{
				return defaultValue;
			}
		}

		private static string ReadString(string rule)
		{
			foreach (var line in _fileContents.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None))
			{
				if (line.ToLower().StartsWith(rule.ToLower() + "="))
				{
					var value = line.Split('=')[1];

					if (rule.ToLower() == "motd") return value; //Do not lower

					return value.ToLower();
				}
			}
			throw new EntryPointNotFoundException("The specified property was not found.");
		}

		private static int ReadInt(string rule, int Default)
		{
			try
			{
				return Convert.ToInt32(ReadString(rule));
			}
			catch
			{
				return Default;
			}
		}

		private static bool ReadBoolean(string rule, bool Default)
		{
			try
			{
				var d = ReadString(rule);
				return Convert.ToBoolean(d);
			}
			catch
			{
				return Default;
			}
		}

		private static Gamemode ReadGamemode(string rule, Gamemode Default)
		{
			try
			{
				var gm = ReadString(rule);
				switch (gm)
				{
					case "1":
					case "creative":
						return Gamemode.Creative;
					case "0":
					case "survival":
						return Gamemode.Survival;
					case "2":
					case "adventure":
						return Gamemode.Adventure;
					default:
						return Default;
				}
			}
			catch
			{
				return Default;
			}
		}

		public static void SetProperty(string property, string value)
		{
			var lined = "";
			foreach (var line in _fileContents.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None))
			{
				if (line.ToLower().StartsWith(property.ToLower() + "="))
				{
					lined = line;
					break; //Correct line is found
				}
			}

			if (lined != "")
			{
				_fileContents.Replace(lined, string.Format("{0}={1}", lined.Split('=')[0], value));
			}
			else
			{
				_fileContents += string.Format("{0}={1}\r\n", property, value);
			}
			ConfigChanged = true;
		}

		public static void SaveConfig()
		{
			if (ConfigChanged)
			{
				File.WriteAllText(ConfigFile, _fileContents);
			}
		}
	}
}