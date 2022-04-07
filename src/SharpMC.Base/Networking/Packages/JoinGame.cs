using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class JoinGame : Package<JoinGame>
	{
		public Player Player;

		public JoinGame(ClientWrapper client) : base(client)
		{
			SendId = 0x01;
		}

		public JoinGame(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x01;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteInt(Player.EntityId);
				Buffer.WriteByte((byte) Player.Gamemode);
				Buffer.WriteByte(Player.Dimension);
				Buffer.WriteByte((byte) Client.Player.Level.Difficulty);
				Buffer.WriteByte((byte) ServerSettings.MaxPlayers);
				Buffer.WriteString(Client.Player.Level.LevelType.ToString());
				Buffer.WriteBool(false);
				Buffer.FlushData();
			}
		}
	}
}