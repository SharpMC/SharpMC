using System.Security.Cryptography;
using System.Text;

namespace SharpMC.Core.Utils
{
	public class JavaHex
	{
		private static string JavaHexDigest(string data)
		{
			var sha1 = SHA1.Create();
			var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
			var negative = (hash[0] & 0x80) == 0x80;
			if (negative) // check for negative hashes
				hash = TwosCompliment(hash);
			// Create the string and trim away the zeroes
			var digest = GetHexString(hash).TrimStart('0');
			if (negative)
				digest = "-" + digest;
			return digest;
		}

		private static string GetHexString(byte[] p)
		{
			var result = string.Empty;
			for (var i = 0; i < p.Length; i++)
				result += p[i].ToString("x2"); // Converts to hex string
			return result;
		}

		private static byte[] TwosCompliment(byte[] p) // little endian
		{
			int i;
			var carry = true;
			for (i = p.Length - 1; i >= 0; i--)
			{
				p[i] = (byte) ~p[i];
				if (carry)
				{
					carry = p[i] == 0xFF;
					p[i]++;
				}
			}
			return p;
		}
	}
}