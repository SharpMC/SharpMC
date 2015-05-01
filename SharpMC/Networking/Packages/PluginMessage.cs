using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PluginMessage : Package<PluginMessage>
	{
		public PluginMessage(ClientWrapper client) : base(client)
		{
			ReadId = 0x17;
		}

		public PluginMessage(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x17;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var message = Buffer.ReadString();

				switch (message)
				{
					case "MC|Brand":
						ConsoleFunctions.WriteInfoLine(Client.Player.Username + "'s client: " + Buffer.ReadString());
						break;
				}
			}
		}
	}
}