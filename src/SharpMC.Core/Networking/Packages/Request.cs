using Newtonsoft.Json;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class Request : Package<Request>
	{
		public Request(ClientWrapper client) : base(client)
		{
			ReadId = 0x00;
		}

		public Request(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x00;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var status = new StatusRequestMessage(Globals.ProtocolName, Globals.ProtocolVersion, ServerSettings.MaxPlayers, Globals.GetOnlinePlayerCount(), ServerSettings.Motd);
				var statusstring = JsonConvert.SerializeObject(status);
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString(statusstring);
				Buffer.FlushData();
			}
		}
	}
}
