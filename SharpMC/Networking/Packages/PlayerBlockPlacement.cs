using System;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.Items;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	internal class PlayerBlockPlacement : Package<PlayerBlockPlacement>
	{
		public PlayerBlockPlacement(ClientWrapper client) : base(client)
		{
			ReadId = 0x08;
		}

		public PlayerBlockPlacement(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x08;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var position = Buffer.ReadPosition();

				if (position.Y > 256)
				{
					return;
				}

				var face = Buffer.ReadByte();

				switch (face)
				{
					case 0:
						position.Y--;
						break;
					case 1:
						position.Y++;
						break;
					case 2:
						position.Z--;
						break;
					case 3:
						position.Z++;
						break;
					case 4:
						position.X--;
						break;
					case 5:
						position.X++;
						break;
				}

				var heldItem = Buffer.ReadUShort();
				if (heldItem <= UInt16.MinValue || heldItem >= UInt16.MaxValue) return;

				var itemCount = Buffer.ReadByte();
				var itemDamage = Buffer.ReadByte();
				var itemMeta = (byte) Buffer.ReadByte();

				var CursorX = Buffer.ReadByte(); //Unused
				var CursorY = Buffer.ReadByte(); //Unused
				var CursorZ = Buffer.ReadByte(); //Unused

				if (Client.Player.Level.GetBlock(position).Id == 0 || Client.Player.Level.GetBlock(position).IsReplacible)
				{
					if (Client.Player.Inventory.HasItem(heldItem) || Client.Player.Gamemode == Gamemode.Creative)
					{
						if (ItemFactory.GetItemById((short) heldItem).IsUsable)
						{
							ItemFactory.GetItemById((short) heldItem).UseItem(Client.Player.Level, Client.Player, position, (BlockFace)face);
							return;
						}

						var b = BlockFactory.GetBlockById(heldItem);
						b.Coordinates = position;
						b.Metadata = itemMeta;
						Client.Player.Level.SetBlock(b, true, heldItem == 8 || heldItem == 10);

						if (Client.Player.Gamemode != Gamemode.Creative)
						{
								Client.Player.Inventory.RemoveItem((short) b.Id, itemMeta, 1);
						}
					}
					else
					{
						Client.Player.Inventory.SendToPlayer(); //Client not synced up, SYNC!
					}
				}
			}
		}
	}
}