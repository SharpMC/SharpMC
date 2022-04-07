using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login
{
	public class EncryptionRequestPacket : Packet<EncryptionRequestPacket>
	{
		public string ServerId { get; set; }
		public byte[] PublicKey { get; set; }
		public byte[] VerifyToken { get; set; }

		public EncryptionRequestPacket()
		{
			PacketId = 0x01;
		}

		public override void Decode(MinecraftStream stream)
		{
			ServerId = stream.ReadString();
			var publicKeyLength = stream.ReadVarInt();
			PublicKey = stream.Read(publicKeyLength);
			var verifyTokenLength = stream.ReadVarInt();
			VerifyToken = stream.Read(verifyTokenLength);
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteString(ServerId);
			stream.WriteVarInt(PublicKey.Length);
			stream.Write(PublicKey);
			stream.WriteVarInt(VerifyToken.Length);
			stream.Write(VerifyToken);
		}
	}
}