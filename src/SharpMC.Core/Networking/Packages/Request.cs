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
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString("{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " +
								   Globals.ProtocolVersion + "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " +
								   Globals.GetOnlineCount() + "},\"description\": {\"text\":\"" +
								   Globals.CleanForJson(Globals.RandomMOTD) +
								   "\"}}");
				Buffer.FlushData();
			}
		}
	}
}
