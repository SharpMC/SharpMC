using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class CollectItem : Package<CollectItem>
	{
		public int CollectorEntityId;
		public int EntityId;

		public CollectItem(ClientWrapper client) : base(client)
		{
			SendId = 0x0D;
		}

		public CollectItem(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
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