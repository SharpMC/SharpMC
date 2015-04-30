using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class PlayerPosition : Package<PlayerPosition>
	{
		public PlayerPosition(ClientWrapper client) : base(client)
		{
			ReadId = 0x04;
		}

		public PlayerPosition(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x04;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var originalCoordinates = Client.Player.KnownPosition;

				var X = Buffer.ReadDouble();
				var FeetY = Buffer.ReadDouble();
				var Z = Buffer.ReadDouble();
				var OnGround = Buffer.ReadBool();
				Client.Player.KnownPosition = new PlayerLocation(X, FeetY, Z);
				Client.Player.KnownPosition.OnGround = OnGround;
				Client.Player.SendChunksFromPosition();

				var movement = Client.Player.KnownPosition - originalCoordinates;
				new EntityRelativeMove(Client) {Player = Client.Player, Movement = movement}.Broadcast(false, Client.Player);
			}
		}
	}
}