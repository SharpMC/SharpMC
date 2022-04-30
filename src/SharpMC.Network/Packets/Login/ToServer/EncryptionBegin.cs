using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToServer
{
    public class EncryptionBegin : Packet<EncryptionBegin>, IToServer
    {
        public byte ServerId => 0x01;

        public byte[] SharedSecret { get; set; }
        public byte[] VerifyToken { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            var sharedSecretLength = stream.ReadVarInt();
            SharedSecret = stream.Read(sharedSecretLength);
            var verifyTokenLength = stream.ReadVarInt();
            VerifyToken = stream.Read(verifyTokenLength);
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(SharedSecret.Length);
            stream.Write(SharedSecret);
            stream.WriteVarInt(VerifyToken.Length);
            stream.Write(VerifyToken);
        }
    }
}