using System;
using System.IO;
using SharpMC.Enums;

namespace SharpMC.Utils
{
	public class Config
	{
		public static string ConfigFile = string.Empty;
		private static string FileContents = string.Empty;
		public static string[] InitialValue;

		public static bool Check()
		{
			if (!File.Exists(ConfigFile))
			{
				File.WriteAllLines(ConfigFile, InitialValue);
				return Check();
			}
			FileContents = File.ReadAllText(ConfigFile);
			if (!FileContents.Contains("#DO NOT REMOVE THIS LINE - SharpMC Config"))
			{
				File.Delete(ConfigFile);
				return Check();
			}
			return true;
		}

		public static Gamemode GetProperty(string Property, Gamemode DefaultValue)
		{
			return ReadGamemode(Property, DefaultValue);
		}

		public static Boolean GetProperty(string Property, Boolean DefaultValue)
		{
			return ReadBoolean(Property, DefaultValue);
		}

		public static int GetProperty(string Property, int DefaultValue)
		{
			return ReadInt(Property, DefaultValue);
		}

		//	public static Difficulty GetProperty(string Property, Difficulty DefaultValue)
		//	{
		//		return ReadDifficulty(Property, DefaultValue);
//}

		public static string GetProperty(string Property, string DefaultValue)
		{
			try
			{
				return ReadString(Property);
			}
			catch
			{
				return DefaultValue;
			}
		}

		private static string ReadString(string Rule)
		{
			foreach (var Line in FileContents.Split(new[] {"\r\n", "\n", Environment.NewLine}, StringSplitOptions.None))
			{
				if (Line.ToLower().StartsWith(Rule.ToLower() + "="))
				{
					var Value = Line.Split('=')[1];

					if (Rule.ToLower() == "motd") return Value; //Do not lower

					return Value.ToLower();
				}
			}
			throw new EntryPointNotFoundException("The specified property was not found.");
		}

		private static int ReadInt(string Rule, int Default)
		{
			try
			{
				return Convert.ToInt32(ReadString(Rule));
			}
			catch
			{
				return Default;
			}
		}

		private static bool ReadBoolean(string Rule, Boolean Default)
		{
			try
			{
				var D = ReadString(Rule);
				return Convert.ToBoolean(D);
			}
			catch
			{
				return Default;
			}
		}

		private static Gamemode ReadGamemode(string Rule, Gamemode Default)
		{
			try
			{
				var gm = ReadString(Rule);
				switch (gm)
				{
					case "1":
					case "creative":
						return Gamemode.Creative;
					case "0":
					case "survival":
						return Gamemode.Surival;
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
	}
}