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
namespace SharpMC.Core.Enums
{
	public class ChatColor
	{
		public char Value = 'r';

		public ChatColor(char value)
		{
			Value = value;
		}

		public static ChatColor
			Green = new ChatColor('a'),
			Aqua = new ChatColor('b'),
			Red = new ChatColor('c'),
			Pink = new ChatColor('d'),
			Yellow = new ChatColor('e'),
			White = new ChatColor('f'),
			Obfuscated = new ChatColor('k'),
			Bold = new ChatColor('l'),
			StrikeThrough = new ChatColor('m'),
			Underline = new ChatColor('n'),
			Italic = new ChatColor('o'),
			Reset = new ChatColor('r'),
			Black = new ChatColor('0'),
			DarkBlue = new ChatColor('1'),
			DarkGreen = new ChatColor('2'),
			DarkAqua = new ChatColor('3'),
			DarkRed = new ChatColor('4'),
			DarkPurple = new ChatColor('5'),
			Gold = new ChatColor('6'),
			Gray = new ChatColor('7'),
			DarkGray = new ChatColor('8'),
			Indigo = new ChatColor('9');
	}
}
