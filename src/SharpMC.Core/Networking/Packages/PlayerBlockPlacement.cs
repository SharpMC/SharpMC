// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using SharpMC.Core.Blocks;
using SharpMC.Core.Enums;
using SharpMC.Core.Items;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerBlockPlacement : Package<PlayerBlockPlacement>
	{
		public PlayerBlockPlacement(ClientWrapper client) : base(client)
		{
			ReadId = 0x08;
		}

		public PlayerBlockPlacement(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x08;
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
				if (heldItem <= ushort.MinValue || heldItem >= ushort.MaxValue) return;

				var itemCount = Buffer.ReadByte();
				var itemDamage = Buffer.ReadByte();
				var itemMeta = (byte) Buffer.ReadByte();

				var cursorX = Buffer.ReadByte(); //Unused
				var cursorY = Buffer.ReadByte(); //Unused
				var cursorZ = Buffer.ReadByte(); //Unused

				
				var blockatloc = Client.Player.Level.GetBlock(c);
				if (blockatloc.IsUsable && !Client.Player.IsCrouching)
				{
					blockatloc.UseItem(Client.Player.Level, Client.Player, c, (BlockFace)face);
					return;
				}

				if (Client.Player.Level.GetBlock(position).Id == 0 || Client.Player.Level.GetBlock(position).IsReplacible)
				{
					if (Client.Player.Inventory.HasItem(heldItem) || Client.Player.Gamemode == Gamemode.Creative)
					{
						/*if (ItemFactory.GetItemById((short) heldItem).IsUsable)
						{
							ItemFactory.GetItemById((short) heldItem).UseItem(Client.Player.Level, Client.Player, position, (BlockFace) face);
							return;
						}*/

						var b = BlockFactory.GetBlockById(heldItem);
						b.Coordinates = position;
						b.Metadata = itemMeta;
						if (!b.PlaceBlock(Client.Player.Level, Client.Player, position, (BlockFace) face))
						{
							Client.Player.Level.SetBlock(b);
						}
						//Client.Player.Level.SetBlock(b, true, heldItem == 8 || heldItem == 10);

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