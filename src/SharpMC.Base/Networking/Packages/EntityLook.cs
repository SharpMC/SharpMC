using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class EntityLook : Package<EntityLook>
	{
		public int EntityId;
		public double Yaw;
		public double Pitch;
		public bool OnGround;

		public EntityLook(ClientWrapper client) : base(client)
		{
			SendId = 0x16;
		}

		public EntityLook(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x16;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteByte((byte)(Yaw / 360 * 256));
				Buffer.WriteByte((byte)Pitch);
				Buffer.WriteBool(OnGround);
				Buffer.FlushData();
			}
		}
	}
}