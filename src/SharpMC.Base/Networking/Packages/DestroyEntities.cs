using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class DestroyEntities : Package<DestroyEntities>
	{
		public int[] EntityIds;

		public DestroyEntities(ClientWrapper client) : base(client)
		{
			SendId = 0x13;
		}

		public DestroyEntities(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x13;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityIds.Length);
				foreach (var i in EntityIds)
				{
					Buffer.WriteVarInt(i);
				}
				Buffer.FlushData();
			}
		}
	}
}