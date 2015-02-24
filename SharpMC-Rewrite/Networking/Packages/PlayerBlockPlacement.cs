using System;
using System.ComponentModel;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Enums;
using SharpMCRewrite.Items;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
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
			var position = Buffer.ReadIntPosition();
			Client.Player.SendChat("Position: " + position.GetString());

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

			/*if (position.X == -1 && position.Y == 256 && position.Z == -1)
			{
				ItemFactory.GetItemById(heldItem, itemMeta)
					.UseItem(Globals.Level, Client.Player, new Vector3(position.X, position.Y, position.Z), (BlockFace) face);
				return;
			}*/

			var b = BlockFactory.GetBlockById(heldItem);
			b.Coordinates = position;
			b.Metadata = itemMeta;
			Globals.Level.SetBlock(b, true);
		}
	}
}