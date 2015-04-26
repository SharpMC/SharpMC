using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class PlayerPositionAndLook : Package<PlayerPositionAndLook>
	{
		public PlayerPositionAndLook(ClientWrapper client) : base(client)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public PlayerPositionAndLook(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteDouble(Globals.Level.Generator.GetSpawnPoint().X);
				Buffer.WriteDouble(Globals.Level.Generator.GetSpawnPoint().Y);
				Buffer.WriteDouble(Globals.Level.Generator.GetSpawnPoint().Z);
				Buffer.WriteFloat(0f);
				Buffer.WriteFloat(0f);
				Buffer.WriteByte(111);
				Buffer.FlushData();
			}
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var originalCoordinates = Client.Player.Coordinates;

				var X = Buffer.ReadDouble();
				var FeetY = Buffer.ReadDouble();
				var Z = Buffer.ReadDouble();
				var Yaw = Buffer.ReadFloat();
				var Pitch = Buffer.ReadFloat();
				var OnGround = Buffer.ReadBool();

				Client.Player.OnGround = OnGround;
				Client.Player.Yaw = Yaw;
				Client.Player.Pitch = Pitch;
				Client.Player.Coordinates = new Vector3(X, FeetY, Z);

				var movement = Client.Player.Coordinates - originalCoordinates;
				new EntityRelativeMove(Client) {Player = Client.Player, Movement = movement}.Broadcast(false, Client.Player);
			}
		}
	}
}