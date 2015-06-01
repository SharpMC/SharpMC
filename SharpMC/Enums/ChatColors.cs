namespace SharpMC.Enums
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
