using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login
{
	public class EncryptionResponsePacket : Packet<EncryptionResponsePacket>
	{
		public byte[] SharedSecret { get; set; }
		public byte[] VerifyToken { get; set; }

		public EncryptionResponsePacket()
		{
			PacketId = 0x01;
		}

		public override void Decode(MinecraftStream stream)
		{
			var sharedSecretLength = stream.ReadVarInt();
			SharedSecret = stream.Read(sharedSecretLength);
			var verifyTokenLength = stream.ReadVarInt();
			VerifyToken = stream.Read(verifyTokenLength);
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteVarInt(SharedSecret.Length);
			stream.Write(SharedSecret);
			stream.WriteVarInt(VerifyToken.Length);
			stream.Write(VerifyToken);
		}
	}
}