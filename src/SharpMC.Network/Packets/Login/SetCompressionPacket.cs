using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login
{
	public class SetCompressionPacket : Packet<SetCompressionPacket>
	{
		public int Threshold;
		public SetCompressionPacket()
		{
			PacketId = 0x03;
		}

		public override void Decode(MinecraftStream stream)
		{
			Threshold = stream.ReadVarInt();
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteVarInt(Threshold);
		}
	}
}
