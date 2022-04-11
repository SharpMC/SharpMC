using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using SharpMC.Logging;
using SharpMC.Util.Encryption;

namespace SharpMC.Util
{
    public class EncryptionHolder
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(EncryptionHolder));

        private RSACryptoServiceProvider Rsa { get; }
        private readonly RSAParameters _privateKey;
        private readonly RSAParameters _publicKey;

        public byte[] PublicKey { get; }

        public EncryptionHolder()
        {
            Rsa = new RSACryptoServiceProvider(1024);
            _privateKey = Rsa.ExportParameters(true);
            _publicKey = Rsa.ExportParameters(false);
            PublicKey = AsnKeyBuilder.PublicKeyToX509(_publicKey).GetBytes();
        }

        public byte[] Decrypt(byte[] data)
        {
            return RsaDecrypt(data, _privateKey, false);
        }

        public static byte[] RsaEncrypt(byte[] dataToEncrypt, RSAParameters rsaKeyInfo,
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
                Log.LogError(e, nameof(RsaEncrypt));
                return null;
            }
        }

        public static byte[] RsaDecrypt(byte[] dataToDecrypt, RSAParameters rsaKeyInfo,
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
                Log.LogError(e, nameof(RsaDecrypt));
                return null;
            }
        }
    }
}