using System;

namespace SharpMC
{
	internal class ConsoleFunctions
	{
		public static void WriteLine(string Text)
		{
			Console.WriteLine(Text);
		}

		public static void WriteLine(string Text, ConsoleColor ForeGroundColor)
		{
			Console.ForegroundColor = ForeGroundColor;
			Console.WriteLine(Text);
			Console.ResetColor();
		}

		public static void WriteLine(string Text, ConsoleColor ForeGroundColor, ConsoleColor BackGroundColor)
		{
			Console.ForegroundColor = ForeGroundColor;
			Console.BackgroundColor = BackGroundColor;
			Console.WriteLine(Text);
			Console.ResetColor();
		}

		public static void WriteInfoLine(string Text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("[INFO] ");
			Console.ResetColor();
			Console.Write(Text + "\n");
		}

		public static void WriteErrorLine(string Text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[ERROR] ");
			Console.ResetColor();
			Console.Write(Text + "\n");
		}

		public static void WriteWarningLine(string Text)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Write("[WARNING] ");
			Console.ResetColor();
			Console.Write(Text + "\n");
		}

		public static void WriteServerLine(string Text)
		{
			//Console.ForegroundColor = ConsoleColor.Green;
			//Console.Write("[SERVER] ");
			//Console.ResetColor();
			//Console.Write(Text + "\n");
			WriteInfoLine(Text);
		}

		public static void WriteDebugLine(string Text)
		{
			 if (Globals.Debug)
			 {
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.Write("[DEBUG] ");
				Console.ResetColor();
				Console.Write(Text + "\n");
			 }
		}
	}
}