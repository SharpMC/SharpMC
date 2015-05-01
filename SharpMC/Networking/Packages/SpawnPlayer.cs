using System;
using SharpMC.Entity;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class SpawnPlayer : Package<SpawnPlayer>
	{
		public Player Player;

		public SpawnPlayer(ClientWrapper client) : base(client)
		{
			SendId = 0x0C;
		}

		public SpawnPlayer(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x0C;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(Player.EntityId);
				Buffer.WriteUUID(new Guid(Player.Uuid));
				Buffer.WriteInt((int) Player.KnownPosition.X*32);
				Buffer.WriteInt((int)Player.KnownPosition.Y * 32);
				Buffer.WriteInt((int)Player.KnownPosition.Z * 32);
				Buffer.WriteByte((byte)Player.KnownPosition.Yaw);
				Buffer.WriteByte((byte)Player.KnownPosition.Pitch);
				Buffer.WriteShort(0);
				Buffer.WriteByte(127);
				Buffer.FlushData();
			}
		}
	}
}