using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class UpdateHealth : Package<UpdateHealth>
	{
		public UpdateHealth(ClientWrapper client) : base(client)
		{
			SendId = 0x06;
		}

		public UpdateHealth(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x06;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteFloat(Client.Player.HealthManager.Health);
				Buffer.WriteVarInt(Client.Player.HealthManager.Food);
				Buffer.WriteFloat(-0.1f);
				Buffer.FlushData();
			}
		}
	}
}