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

namespace SharpMC.Enums
{
	public class ChatColor
	{
		public static ChatColor Green = new ChatColor('a');

		public static ChatColor Aqua = new ChatColor('b');

		public static ChatColor Red = new ChatColor('c');

		public static ChatColor Pink = new ChatColor('d');

		public static ChatColor Yellow = new ChatColor('e');

		public static ChatColor White = new ChatColor('f');

		public static ChatColor Obfuscated = new ChatColor('k');

		public static ChatColor Bold = new ChatColor('l');

		public static ChatColor StrikeThrough = new ChatColor('m');

		public static ChatColor Underline = new ChatColor('n');

		public static ChatColor Italic = new ChatColor('o');

		public static ChatColor Reset = new ChatColor('r');

		public static ChatColor Black = new ChatColor('0');

		public static ChatColor DarkBlue = new ChatColor('1');

		public static ChatColor DarkGreen = new ChatColor('2');

		public static ChatColor DarkAqua = new ChatColor('3');

		public static ChatColor DarkRed = new ChatColor('4');

		public static ChatColor DarkPurple = new ChatColor('5');

		public static ChatColor Gold = new ChatColor('6');

		public static ChatColor Gray = new ChatColor('7');

		public static ChatColor DarkGray = new ChatColor('8');

		public static ChatColor Indigo = new ChatColor('9');

		public char Value = 'r';

		public ChatColor(char value)
		{
			this.Value = value;
		}
	}
}