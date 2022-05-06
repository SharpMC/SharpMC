using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.Util.Encryption;

namespace SharpMC.Util
{
    internal sealed class EncryptionHolder : IEncryption
    {
        private readonly ILogger _log;

        private RSACryptoServiceProvider Rsa { get; }
        private readonly RSAParameters _privateKey;
        private readonly RSAParameters _publicKey;

        public byte[] PublicKey { get; }

        public EncryptionHolder(ILogger<EncryptionHolder> log)
        {
            _log = log;
            Rsa = new RSACryptoServiceProvider(1024);
            _privateKey = Rsa.ExportParameters(true);
            _publicKey = Rsa.ExportParameters(false);
            PublicKey = AsnKeyBuilder.PublicKeyToX509(_publicKey).GetBytes();
        }

        public byte[] Decrypt(byte[] data)
        {
            return RsaDecrypt(data, _privateKey, false);
        }

        private byte[] RsaEncrypt(byte[] dataToEncrypt, RSAParameters rsaKeyInfo,
            bool doOaepPadding)
        {
            try
            {
                byte[] encryptedData;
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaKeyInfo);
                    encryptedData = rsa.Encrypt(dataToEncrypt, doOaepPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                _log.LogError(e, nameof(RsaEncrypt));
                return null;
            }
        }

        private byte[] RsaDecrypt(byte[] dataToDecrypt, RSAParameters rsaKeyInfo,
            bool doOaepPadding)
        {
            try
            {
                byte[] decryptedData;
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaKeyInfo);
                    decryptedData = rsa.Decrypt(dataToDecrypt, doOaepPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                _log.LogError(e, nameof(RsaDecrypt));
                return null;
            }
        }
    }
}