using SharpMC.Core.Utils;

namespace SharpMC.Core.PluginChannel
{
	public class BrandMessage : PluginMessage
	{
		public BrandMessage() : base("Brand")
		{
			
		}

		public override void HandleData(ClientWrapper client, DataBuffer buffer)
		{
			string c = buffer.ReadString();
			ConsoleFunctions.WriteInfoLine(client.Player.Username + "'s client: " + c);
		}
	}
}
