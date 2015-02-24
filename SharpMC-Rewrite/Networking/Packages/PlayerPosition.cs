using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
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
			var originalCoordinates = Client.Player.Coordinates;

			var X = Buffer.ReadDouble();
			var FeetY = Buffer.ReadDouble();
			var Z = Buffer.ReadDouble();
			var OnGround = Buffer.ReadBool();
			Client.Player.Coordinates = new Vector3(X, FeetY, Z);
			Client.Player.OnGround = OnGround;
			Client.Player.SendChunksFromPosition();

			var movement = Client.Player.Coordinates - originalCoordinates;
			new EntityRelativeMove(Client) {Player = Client.Player, Movement = movement}.Broadcast(false, Client.Player);
		}
	}
}