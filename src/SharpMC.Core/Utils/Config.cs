// Distrubuted under the MIT license
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
using SharpMC.Core.Enums;

namespace SharpMC.Core.Utils
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

		public static bool GetProperty(string Property, bool DefaultValue)
		{
			return ReadBoolean(Property, DefaultValue);
		}

		public static int GetProperty(string Property, int DefaultValue)
		{
			return ReadInt(Property, DefaultValue);
		}

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

		private static bool ReadBoolean(string Rule, bool Default)
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
	}
}