// Distributed under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

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
			string lined = "";
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