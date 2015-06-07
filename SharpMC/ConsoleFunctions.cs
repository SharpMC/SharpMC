#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC
{
	using System;

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
			// Console.ForegroundColor = ConsoleColor.Green;
			// Console.Write("[SERVER] ");
			// Console.ResetColor();
			// Console.Write(Text + "\n");
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