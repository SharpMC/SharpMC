using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class PluginMessage : Package<PluginMessage>
	{
		public PluginMessage(ClientWrapper client) : base(client)
		{
			ReadId = 0x18;
		}

		public PluginMessage(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x18;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				Globals.MessageFactory.HandleMessage(Client, Buffer);
			}
		}
	}
}