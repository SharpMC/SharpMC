using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class EncryptionBegin : Packet<EncryptionBegin>, IToClient
    {
        public byte ClientId => 0x01;

        public string ServerId { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] VerifyToken { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ServerId = stream.ReadString();
            var publicKeyLength = stream.ReadVarInt();
            PublicKey = stream.Read(publicKeyLength);
            var verifyTokenLength = stream.ReadVarInt();
            VerifyToken = stream.Read(verifyTokenLength);
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(ServerId);
            stream.WriteVarInt(PublicKey.Length);
            stream.Write(PublicKey);
            stream.WriteVarInt(VerifyToken.Length);
            stream.Write(VerifyToken);
        }
    }
}