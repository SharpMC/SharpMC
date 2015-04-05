using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Enums;

namespace SharpMCRewrite.Networking.Packages
{
	class PlayerDigging : Package<PlayerDigging>
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

					var block = Globals.Level.GetBlock(intVector);
					block.BreakBlock(Globals.Level);
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
