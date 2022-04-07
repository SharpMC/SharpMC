using System.Numerics;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Util;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerDigging : Package<PlayerDigging>
	{
		public PlayerDigging(ClientWrapper client) : base(client)
		{
			ReadId = 0x07;
		}

		public PlayerDigging(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x07;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var status = Buffer.ReadByte();
				var position = Buffer.ReadPosition();
				var face = Buffer.ReadByte();

				if (status == 3)
				{
					//Drop item stack
					Client.Player.Inventory.DropCurrentItemStack();
					return;
				}

				if (status == 4)
				{
					//Drop item
					Client.Player.Inventory.DropCurrentItem();
					return;
				}

				if (status == 6)
				{
					Client.Player.Inventory.SwapHands();
					return;
				}

				if (position == Vector3.Zero) return;

				if (status == 2 || Client.Player.Gamemode == Gamemode.Creative)
				{
					var block = Client.Player.Level.GetBlock(position);
					block.BreakBlock(Client.Player.Level);
					Client.Player.Digging = false;

					if (Client.Player.Gamemode != Gamemode.Creative)
					{
						foreach (var its in block.Drops)
						{
							new ItemEntity(Client.Player.Level, its)
							{
								KnownPosition = new PlayerLocation(position.X, position.Y, position.Z)
							}.SpawnEntity();
						}
					}
				}

				else if (status == 0)
				{
					Client.Player.Digging = true;
				}
				else if (status == 1)
				{
					Client.Player.Digging = false;
				}
			}
		}
	}
}