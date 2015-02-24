using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class JoinGame : Package<JoinGame>
	{
		public Player Player;
		public JoinGame(ClientWrapper client) : base(client)
		{
			SendId = 0x01;
		}

		public JoinGame(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x01;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteInt(Player.UniqueServerID);
			Buffer.WriteByte((byte)Player.Gamemode);
			Buffer.WriteByte((byte)Player.Dimension);
			Buffer.WriteByte((byte)Globals.Level.Difficulty);
			Buffer.WriteByte((byte)Globals.MaxPlayers);
			Buffer.WriteString(Globals.Level.LevelType.ToString());
			Buffer.WriteBool(false);
			Buffer.FlushData();
		}
	}
}
