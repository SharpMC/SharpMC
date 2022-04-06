using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class ChunkDataPacket : Packet<ChunkDataPacket>
	{
		public ChunkDataPacket()
		{
			PacketId = 0x20;
		}

		public byte[] Data;

		public override void Decode(MinecraftStream stream)
		{
			
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.Write(Data);
		}
	}
}
