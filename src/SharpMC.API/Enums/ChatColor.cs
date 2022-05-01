namespace SharpMC.API.Enums
{
    public sealed record ChatColor(char Value)
    {
        public static ChatColor
            Green = new('a'),
            Aqua = new('b'),
            Red = new('c'),
            Pink = new('d'),
            Yellow = new('e'),
            White = new('f'),
            Obfuscated = new('k'),
            Bold = new('l'),
            StrikeThrough = new('m'),
            Underline = new('n'),
            Italic = new('o'),
            Reset = new('r'),
            Black = new('0'),
            DarkBlue = new('1'),
            DarkGreen = new('2'),
            DarkAqua = new('3'),
            DarkRed = new('4'),
            DarkPurple = new('5'),
            Gold = new('6'),
            Gray = new('7'),
            DarkGray = new('8'),
            Indigo = new('9');
    }
}