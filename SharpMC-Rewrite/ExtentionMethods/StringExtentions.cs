using System;

namespace SharpMCRewrite
{
    public static class StringExtensions
    {
        public static string RemoveLineBreaks( this string lines )
        {
            return lines.Replace( "\r", "").Replace( "\n", "" );
        }

        public static string ReplaceLineBreaks( this string lines, string replacement )
        {
            return lines.Replace( "\r\n", replacement )
                .Replace( "\r", replacement )
                .Replace( "\n", replacement );
        }
    }
}

