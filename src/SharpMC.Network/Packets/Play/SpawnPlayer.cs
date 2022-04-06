using System;
using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class SpawnPlayerPacket : Packet<SpawnPlayerPacket>
	{
		public SpawnPlayerPacket()
		{
			PacketId = 0x05;
		}

		public int EntityId;
		public Guid Uuid;
		public double X;
		public double Y;
		public double Z;
		public byte Yaw;
		public byte Pitch;

		public override void Decode(MinecraftStream stream)
		{
			
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteVarInt(EntityId);
			stream.WriteUuid(Uuid);
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteByte(Yaw);
			stream.WriteByte(Pitch);
			stream.WriteByte(0xff); //End of metadata
		}
	}
}
