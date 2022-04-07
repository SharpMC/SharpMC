using System;

namespace SharpMC.Core
{
	internal class ConsoleFunctions
	{
		public static void WriteLine(string text)
		{
			Console.WriteLine(text);
		}

		public static void WriteLine(string text, ConsoleColor foreGroundColor)
		{
			Console.ForegroundColor = foreGroundColor;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void WriteLine(string text, ConsoleColor foreGroundColor, ConsoleColor backGroundColor)
		{
			Console.ForegroundColor = foreGroundColor;
			Console.BackgroundColor = backGroundColor;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void WriteInfoLine(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[INFO] ");
			Console.ResetColor();
			Console.Write(text + "\n");
		}

		public static void WriteErrorLine(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[ERROR] ");
			Console.ResetColor();
			Console.Write(text + "\n");
		}

		public static void WriteWarningLine(string text)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Write("[WARNING] ");
			Console.ResetColor();
			Console.Write(text + "\n");
		}

		public static void WriteServerLine(string text)
		{
			WriteInfoLine(text);
		}

		public static void WriteDebugLine(string text)
		{
			if (ServerSettings.Debug)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("[DEBUG] ");
				Console.ResetColor();
				Console.Write(text + "\n");
			}
		}
	}
}