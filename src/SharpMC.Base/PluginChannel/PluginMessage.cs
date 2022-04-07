using SharpMC.Core.Utils;

namespace SharpMC.PluginChannel
{
	public class PluginMessage
	{
		public string Command { get; private set; }
		public PluginMessage(string command)
		{
			Command = command;
		}

		public virtual void HandleData(ClientWrapper client, DataBuffer buffer)
		{
			
		}
	}
}