using System;
using SharpMC.Core.Utils;
using SharpMC.Enums;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerListItem : Package<PlayerListItem>
	{
		public int Action = 0;
		public Gamemode Gamemode;
		public string Username;
		public string Uuid;
		public int Latency = 0;

		public PlayerListItem(ClientWrapper client) : base(client)
		{
			SendId = 0x38;
		}

		public PlayerListItem(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x38;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(Action);
				Buffer.WriteVarInt(1);
				Buffer.WriteUuid(new Guid(Uuid));
				switch (Action)
				{
					case 0:
						Buffer.WriteString(Username);
						Buffer.WriteVarInt(Latency);
						Buffer.WriteVarInt((byte) Gamemode);
						Buffer.WriteVarInt(0);
						Buffer.WriteBool(false);
						break;
                    case 1:
                        Buffer.WriteVarInt((byte)Gamemode);
                        break;
					case 2:
						Buffer.WriteVarInt(Latency);
						break;
					case 4:
						break;
				}
				Buffer.FlushData();
			}
		}
	}
}