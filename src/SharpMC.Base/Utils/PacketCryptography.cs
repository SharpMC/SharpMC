// Distributed under the MIT license
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

using System;
using System.Security.Cryptography;

namespace SharpMC.Core.Utils
{
	//Code taken from C-Raft. (https://github.com/chraft/c-raft)
	public static class PacketCryptography
	{
		/* This is hardcoded since it won't change. 
		   It's 1 byte Sequence Tag, 1 byte Sequence Length, 1 byte Oid Tag, 1 byte Oid Length, 9 bytes Oid, 1 byte Null Tag, 1 byte Null */

		private static readonly byte[] AlgorithmId =
		{
			0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01,
			0x01, 0x05, 0x00
		};

		private static RSACryptoServiceProvider _provider;
		public static byte[] VerifyToken { get; set; }

		public static string JavaHexDigest(byte[] data)
		{
			var sha1 = SHA1.Create();
			var hash = sha1.ComputeHash(data);
			var negative = (hash[0] & 0x80) == 0x80;
			if (negative) // check for negative hashes
				hash = TwosCompliment(hash);
			// Create the string and trim away the zeroes
			var digest = GetHexString(hash).Trim('0');
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

		public static RSAParameters GenerateKeyPair()
		{
			if (_provider == null)
				_provider = new RSACryptoServiceProvider(1024);

			return _provider.ExportParameters(true);
		}

		public static byte[] Decrypt(byte[] toDecrypt)
		{
			return _provider.Decrypt(toDecrypt, false);
		}

		public static byte[] Encrypt(byte[] toDecrypt)
		{
			return _provider.Encrypt(toDecrypt, false);
		}

		public static RijndaelManaged GenerateAes(byte[] key)
		{
			var cipher = new RijndaelManaged();
			cipher.Mode = CipherMode.CFB;
			cipher.Padding = PaddingMode.None;
			cipher.KeySize = 128;
			cipher.FeedbackSize = 8;

			cipher.Key = key;
			cipher.IV = (byte[]) key.Clone();

			return cipher;
		}

		public static byte[] GetRandomToken()
		{
			var token = new byte[4];
			var provider = new RNGCryptoServiceProvider();
			provider.GetBytes(token);
			VerifyToken = token;
			return token;
		}

		public static byte[] PublicKeyToAsn1(RSAParameters parameters)
		{
			// Oid - Tag: 0x06 - Length: 0x09 - Octets: 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01
			// AlgorithmId - Tag: 0x30 - Length: 0x0D - Octets: 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00

			// mod - Tag: 0x02 - Length: ? - Octets: 0x00?, parameters.Modulus
			// exp - Tag: 0x02 - Length: ? - Octets: 0x00?, parameters.Exponent            
			var mod = CreateIntegerPos(parameters.Modulus);
			var exp = CreateIntegerPos(parameters.Exponent);

			// Sequence(mod + exp) - Tag: 0x30 - Length: ? - Octets: mod + exp
			// Key - Tag: 0x03 - Length: ? - Octets: 0x00, Sequence(mod + exp)
			// PublicKey - Tag: 0x30 - Length: ? - Octets: AlgorithmId + Key
			// AsnMessage - PublicKey

			var sequenceOctetsLength = mod.Length + exp.Length;
			var sequenceLengthArray = LengthToByteArray(sequenceOctetsLength);

			var keyOctetsLength = sequenceLengthArray.Length + sequenceOctetsLength + 2;
			var keyLengthArray = LengthToByteArray(keyOctetsLength);

			var publicKeyOctetsLength = keyOctetsLength + keyLengthArray.Length + AlgorithmId.Length + 1;
			var publicKeyLengthArray = LengthToByteArray(publicKeyOctetsLength);

			var messageLength = publicKeyOctetsLength + publicKeyLengthArray.Length + 1;

			var message = new byte[messageLength];
			var index = 0;

			message[index++] = 0x30;

			Buffer.BlockCopy(publicKeyLengthArray, 0, message, index, publicKeyLengthArray.Length);
			index += publicKeyLengthArray.Length;

			Buffer.BlockCopy(AlgorithmId, 0, message, index, AlgorithmId.Length);

			index += AlgorithmId.Length;

			message[index++] = 0x03;

			Buffer.BlockCopy(keyLengthArray, 0, message, index, keyLengthArray.Length);
			index += keyLengthArray.Length;

			message[index++] = 0x00;
			message[index++] = 0x30;

			Buffer.BlockCopy(sequenceLengthArray, 0, message, index, sequenceLengthArray.Length);
			index += sequenceLengthArray.Length;

			Buffer.BlockCopy(mod, 0, message, index, mod.Length);
			index += mod.Length;
			Buffer.BlockCopy(exp, 0, message, index, exp.Length);

			return message;
		}

		private static byte[] LengthToByteArray(int octetsLength)
		{
			byte[] length = null;

			// Length: 0 <= l < 0x80
			if (octetsLength < 0x80)
			{
				length = new byte[1];
				length[0] = (byte) octetsLength;
			}
			// 0x80 < length <= 0xFF
			else if (octetsLength <= 0xFF)
			{
				length = new byte[2];
				length[0] = 0x81;
				length[1] = (byte) ((octetsLength & 0xFF));
			}

			//
			// We should almost never see these...
			//

			// 0xFF < length <= 0xFFFF
			else if (octetsLength <= 0xFFFF)
			{
				length = new byte[3];
				length[0] = 0x82;
				length[1] = (byte) ((octetsLength & 0xFF00) >> 8);
				length[2] = (byte) ((octetsLength & 0xFF));
			}

			// 0xFFFF < length <= 0xFFFFFF
			else if (octetsLength <= 0xFFFFFF)
			{
				length = new byte[4];
				length[0] = 0x83;
				length[1] = (byte) ((octetsLength & 0xFF0000) >> 16);
				length[2] = (byte) ((octetsLength & 0xFF00) >> 8);
				length[3] = (byte) ((octetsLength & 0xFF));
			}
			// 0xFFFFFF < length <= 0xFFFFFFFF
			else
			{
				length = new byte[5];
				length[0] = 0x84;
				length[1] = (byte) ((octetsLength & 0xFF000000) >> 24);
				length[2] = (byte) ((octetsLength & 0xFF0000) >> 16);
				length[3] = (byte) ((octetsLength & 0xFF00) >> 8);
				length[4] = (byte) ((octetsLength & 0xFF));
			}

			return length;
		}

		private static byte[] CreateIntegerPos(byte[] value)
		{
			byte[] newInt;

			if (value[0] > 0x7F)
			{
				// Integer length + Positive byte
				var length = LengthToByteArray(value.Length + 1);
				var index = 1;
				// Tag + Length + Positive byte + Value
				newInt = new byte[value.Length + 2 + length.Length];
				// Int Tag
				newInt[0] = 0x02;
				// Int Length
				for (var i = 0; i < length.Length; ++i)
					newInt[index++] = length[i];

				// Makes the number positive
				newInt[index++] = 0x00;
				Buffer.BlockCopy(value, 0, newInt, index, value.Length);
			}
			else
			{
				var length = LengthToByteArray(value.Length);
				var index = 1;

				// Tag + Length + Value
				newInt = new byte[value.Length + 1 + length.Length];
				// Int Tag
				newInt[0] = 0x02;
				// Int Length
				for (var i = 0; i < length.Length; ++i)
					newInt[index++] = length[i];

				Buffer.BlockCopy(value, 0, newInt, index, value.Length);
			}

			return newInt;
		}
	}
}