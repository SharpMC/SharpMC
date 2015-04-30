using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	class ClientStatus : Package<ClientStatus>
	{
		public ClientStatus(ClientWrapper client)
			: base(client)
		{
			ReadId = 0x16;
		}

		public ClientStatus(ClientWrapper client, MSGBuffer buffer)
			: base(client, buffer)
		{
			ReadId = 0x16;
		}

		public override void Read()
		{
			var actionId = Buffer.ReadVarInt();
			if (actionId == 0)
			{
				Client.Player.Respawn();
			}
		}
	}
}
