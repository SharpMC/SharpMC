using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class DestroyEntities : Package<DestroyEntities>
	{
		public int[] EntityIds;
		public DestroyEntities(ClientWrapper client) : base(client)
		{
			SendId = 0x13;
		}

		public DestroyEntities(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x13;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityIds.Length);
				foreach (int i in EntityIds)
				{
					Buffer.WriteVarInt(i);
				}
				Buffer.FlushData();
			}
		}
	}
}
