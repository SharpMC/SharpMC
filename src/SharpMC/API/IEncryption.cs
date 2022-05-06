namespace SharpMC.API
{
    public interface IEncryption
    {
        byte[] Decrypt(byte[] input);

        byte[] PublicKey { get; }
    }
}