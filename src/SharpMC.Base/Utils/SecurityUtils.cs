using System;
using System.Security.Cryptography;

namespace SharpMC.Core.Utils
{
	public class SecurityUtils
	{
		private readonly Random _random = new Random();
		private readonly RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider(1024);

		public string GeneratePublicKey(bool privateparameters)
		{
			var publicKeyXml = _rsa.ToXmlString(false);
			Console.WriteLine(publicKeyXml);

			return publicKeyXml;
		}

		public byte[] GenerateVerifyToken()
		{
			var token = new byte[4];
			_random.NextBytes(token);
			return token;
		}
	}
}