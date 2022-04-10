using System.Text;

namespace SharpMC.Generator.Tools
{
    internal static class Helpers
    {
        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            var bld = new StringBuilder();
            foreach (var part in text.Split('_'))
            {
                var big = part.Substring(0, 1)
                    .ToUpperInvariant() + part.Substring(1);
                bld.Append(big);
            }
            var txt = bld.ToString();
            return txt;
        }
    }
}