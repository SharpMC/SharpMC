using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Networking.Packages
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
			var message = Buffer.ReadString();

			ConsoleFunctions.WriteDebugLine("Plugin Message: " + message);
		}
	}
}