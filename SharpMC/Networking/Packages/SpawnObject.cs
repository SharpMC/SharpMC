using SharpMC.Enums;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class SpawnObject : Package<SpawnObject>
	{
		public int EntityId = 0;
		public double X = 0;
		public double Y = 0;
		public double Z = 0;
		public byte Pitch = 0;
		public byte Yaw = 0;
		public ObjectType Type;
		public object Data;
		public short VelocityX = 0;
		public short VelocityY = 0;
		public short VelocityZ = 0;

		public SpawnObject(ClientWrapper client) : base(client)
		{
			SendId = 0x0E;
		}

		public SpawnObject(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0E;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteByte((byte)Type);
				Buffer.WriteInt((int)X * 32);
				Buffer.WriteInt((int)Y * 32);
				Buffer.WriteInt((int)Z * 32);
				Buffer.WriteByte(Pitch);
				Buffer.WriteByte(Yaw);
				if (Type == ObjectType.ItemStack)
				{
					Buffer.WriteInt(0);
				}
				Buffer.FlushData();
			}
		}
	}
}
