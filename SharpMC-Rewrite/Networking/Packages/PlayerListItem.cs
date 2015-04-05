using System;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Enums;

namespace SharpMCRewrite.Networking.Packages
{
	internal class PlayerListItem : Package<PlayerListItem>
	{
		public int Action = 0;
		public Gamemode Gamemode;
		public string Username;
		public string UUID;

		public PlayerListItem(ClientWrapper client) : base(client)
		{
			SendId = 0x38;
		}

		public PlayerListItem(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x38;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteVarInt(Action);
			Buffer.WriteVarInt(1);
			//foreach(Player player in Globals.Level.OnlinePlayers)
			//{
			Buffer.WriteUUID(new Guid(UUID));
			switch (Action)
			{
				case 0:
					Buffer.WriteString(Username);
					Buffer.WriteVarInt(0);
					Buffer.WriteVarInt((byte) Gamemode);
					Buffer.WriteVarInt(0);
					Buffer.WriteBool(false);
					break;
				case 4:
					break;
			}
			//}
			Buffer.FlushData();
		}
	}
}