using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class EntityVelocity : Package<EntityVelocity>
	{
		public int EntityId;
		public short VelocityX;
		public short VelocityY;
		public short VelocityZ;

		public EntityVelocity(ClientWrapper client) : base(client)
		{
			SendId = 0x12;
		}

		public EntityVelocity(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x12;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteShort(VelocityX);
				Buffer.WriteShort(VelocityY);
				Buffer.WriteShort(VelocityZ);
				Buffer.FlushData();
			}
		}
	}
}
