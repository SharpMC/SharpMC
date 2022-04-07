using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class UnloadChunk : Packet<UnloadChunk>
	{
		public UnloadChunk()
		{
			PacketId = 0x1D;
		}

		public int X;
		public int Z;

		public override void Decode(MinecraftStream stream)
		{
			
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteInt(X);
			stream.WriteInt(Z);
		}
	}
}