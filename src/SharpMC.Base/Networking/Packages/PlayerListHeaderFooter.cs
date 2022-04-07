using Newtonsoft.Json;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerListHeaderFooter : Package<PlayerListHeaderFooter>
	{
		public PlayerListHeaderFooter(ClientWrapper client) : base(client)
		{
			SendId = 0x47;
		}

		public PlayerListHeaderFooter(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x47;
		}

		public McChatMessage Header { get; set; }
		public McChatMessage Footer { get; set; }

		public override void Write()
		{
			if (Buffer != null)
			{
				var head = JsonConvert.SerializeObject(Header);
				var foot = JsonConvert.SerializeObject(Footer);
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString(head);
				Buffer.WriteString(foot);
				Buffer.FlushData();
			}
		}
	}
}