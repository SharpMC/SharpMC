using SharpMC.Classes;
using SharpMC.Enums;

namespace SharpMC.Networking.Packages
{
	internal class PlayerDigging : Package<PlayerDigging>
	{
		public PlayerDigging(ClientWrapper client) : base(client)
		{
			ReadId = 0x07;
		}

		public PlayerDigging(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x07;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var status = Buffer.ReadByte();

				if (status == 2 || Client.Player.Gamemode == Gamemode.Creative)
				{
					var Position = Buffer.ReadPosition();
					var Face = Buffer.ReadByte();
					var intVector = new Vector3((int) Position.X, (int) Position.Y, (int) Position.Z);

					var block = Client.Player.CurrentLevel.GetBlock(intVector);
					block.BreakBlock(Client.Player.CurrentLevel);
					//Globals.Level.SetBlock(new BlockAir() {Coordinates = intVector});
					Client.Player.Digging = false;
				}
				else if (status == 0)
				{
					Client.Player.Digging = true;
				}
			}
		}
	}
}