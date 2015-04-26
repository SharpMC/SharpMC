using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class EntityTeleport : Package<EntityTeleport>
	{
		public Vector3 Coordinates;
		public bool OnGround;
		public byte Pitch;
		public int UniqueServerID;
		public byte Yaw;

		public EntityTeleport(ClientWrapper client) : base(client)
		{
			SendId = 0x18;
		}

		public EntityTeleport(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x18;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(UniqueServerID);
				Buffer.WriteInt((int) Coordinates.X*32);
				Buffer.WriteInt((int) Coordinates.Y*32);
				Buffer.WriteInt((int) Coordinates.Z*32);
				Buffer.WriteByte(Yaw);
				Buffer.WriteByte(Pitch);
				Buffer.WriteBool(OnGround);
				Buffer.FlushData();
			}
		}

		public static void Broadcast(Player player)
		{
			foreach (var i in Globals.Level.OnlinePlayers)
			{
				if (i != player)
					new EntityTeleport(i.Wrapper)
					{
						Coordinates = player.Coordinates,
						OnGround = player.OnGround,
						UniqueServerID = player.UniqueServerId,
						Pitch = (byte) player.Pitch,
						Yaw = (byte) player.Yaw
					}.Write();
			}
		}
	}
}