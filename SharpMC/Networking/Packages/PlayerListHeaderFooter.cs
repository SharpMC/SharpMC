using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class PlayerListHeaderFooter : Package<PlayerListHeaderFooter>
	{
		public PlayerListHeaderFooter(ClientWrapper client) : base(client)
		{
			SendId = 0x47;
		}

		public PlayerListHeaderFooter(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x47;
		}

		public string Header { get; set; }
		public string Footer { get; set; }

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteString("{ \"text\": \"" + Header + "\" }");
			Buffer.WriteString("{ \"text\": \"" + Footer + "\" }");
			Buffer.FlushData();
		}
	}
}