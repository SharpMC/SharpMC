using System;
using SharpMC.Core.Utils;
using SharpMC.Enums;

namespace SharpMC.Core.Networking.Packages
{
	public class SpawnObject : Package<SpawnObject>
	{
		public int Info = 0;
		public int EntityId = 0;
		public double Pitch = 0;
		public ObjectType Type;
		public short VelocityX = 0;
		public short VelocityY = 0;
		public short VelocityZ = 0;
		public double X = 0;
		public double Y = 0;
		public double Yaw = 0;
		public double Z = 0;
		public Guid ObjectUuid = new Guid();

		public SpawnObject(ClientWrapper client) : base(client)
		{
			SendId = 0x0E;
		}

		public SpawnObject(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0E;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				//ConsoleFunctions.WriteInfoLine("Spawning object with Pitch: " + Pitch + ", Yaw: " + Yaw);
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteUuid(ObjectUuid);
				Buffer.WriteByte((byte) Type);
				Buffer.WriteInt((int) X*32);
				Buffer.WriteInt((int) Y*32);
				Buffer.WriteInt((int) Z*32);
				Buffer.WriteByte((byte)Pitch);
				Buffer.WriteByte((byte)(Yaw/360*256));
				Buffer.WriteInt((int) Info);
				Buffer.WriteShort(VelocityX);
				Buffer.WriteShort(VelocityY);
				Buffer.WriteShort(VelocityZ);

				Buffer.FlushData();
			}
		}
	}
}