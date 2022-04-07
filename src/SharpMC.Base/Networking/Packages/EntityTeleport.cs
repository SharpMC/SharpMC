using System.Numerics;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class EntityTeleport : Package<EntityTeleport>
	{
		public Vector3 Coordinates;
		public bool OnGround;
		public double Pitch;
		public int UniqueServerId;
		public double Yaw;

		public EntityTeleport(ClientWrapper client) : base(client)
		{
			SendId = 0x18;
		}

		public EntityTeleport(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x18;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(UniqueServerId);
				Buffer.WriteInt((int) Coordinates.X*32);
				Buffer.WriteInt((int) Coordinates.Y*32);
				Buffer.WriteInt((int) Coordinates.Z*32);
				Buffer.WriteByte((byte)(Yaw / 360 * 256));
				Buffer.WriteByte((byte)Pitch);
				Buffer.WriteBool(OnGround);
				Buffer.FlushData();
			}
		}
	}
}