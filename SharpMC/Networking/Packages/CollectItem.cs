using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class CollectItem : Package<CollectItem>
	{
		public int EntityId;
		public int CollectorEntityId;

		public CollectItem(ClientWrapper client) : base(client)
		{
			SendId = 0x0D;
		}

		public CollectItem(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0D;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteVarInt(CollectorEntityId);
				Buffer.FlushData();
			}
		}
	}
}
