using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Core.Utils;
using SharpMC.Enums;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerBlockPlacement : Package<PlayerBlockPlacement>
	{
		public PlayerBlockPlacement(ClientWrapper client) : base(client)
		{
			ReadId = 0x09;
		}

		public PlayerBlockPlacement(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x09;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var position = Buffer.ReadPosition();
				Vector3 c = new Vector3(position.X, position.Y, position.Z);

				if (position.Y > 256)
				{
					return;
				}

				var face = Buffer.ReadVarInt();

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

				//var heldItem = Buffer.ReadUShort();
				//if (heldItem <= ushort.MinValue || heldItem >= ushort.MaxValue) return;

				//var itemCount = Buffer.ReadByte();
				//var itemDamage = Buffer.ReadByte();
				//var itemMeta = (byte) Buffer.ReadByte();
				var hand = Buffer.ReadVarInt();

				var cursorX = Buffer.ReadByte(); //Unused
				var cursorY = Buffer.ReadByte(); //Unused
				var cursorZ = Buffer.ReadByte(); //Unused

				
				var blockatloc = Client.Player.Level.GetBlock(c);
				if (blockatloc.IsUsable && !Client.Player.IsCrouching)
				{
					blockatloc.UseItem(Client.Player.Level, Client.Player, c, (BlockFace)face);
					return;
				}

					var d = Client.Player.Inventory.GetItemInHand(hand);
					if (d.IsBlock)
					{
						var f = (Block) d;
						f.Coordinates = position;
						if (!f.PlaceBlock(Client.Player.Level, Client.Player, c, (BlockFace) face, new Vector3(cursorX, cursorY, cursorZ)))
						{
							Client.Player.Level.SetBlock(f);
						}

						if (Client.Player.Gamemode != Gamemode.Creative)
						{
							Client.Player.Inventory.RemoveItem((short)f.Id, f.Metadata, 1);
						}
					}
					else
					{
						if (d.IsUsable)
						{
							d.UseItem(Client.Player.Level, Client.Player, c, (BlockFace)face);

							if (Client.Player.Gamemode != Gamemode.Creative)
							{
								Client.Player.Inventory.RemoveItem((short)d.Id, d.Metadata, 1);
							}
						}
					}
			}
		}
	}
}