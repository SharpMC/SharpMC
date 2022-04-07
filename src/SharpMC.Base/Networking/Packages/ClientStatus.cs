using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class ClientStatus : Package<ClientStatus>
	{
		public ClientStatus(ClientWrapper client)
			: base(client)
		{
			ReadId = 0x17;
		}

		public ClientStatus(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			ReadId = 0x17;
		}

		public override void Read()
		{
			var actionId = Buffer.ReadVarInt();
			//0 = Perform respawn
			//1 = Request stats
			//2 = Open Inventory
			if (actionId == 0)
			{
				Client.Player.Respawn();
			}
		}
	}
}