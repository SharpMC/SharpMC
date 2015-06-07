#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Networking.Packages
{
	using SharpMC.Blocks;
	using SharpMC.Enums;
	using SharpMC.Items;
	using SharpMC.Utils;

	internal class PlayerBlockPlacement : Package<PlayerBlockPlacement>
	{
		public PlayerBlockPlacement(ClientWrapper client)
			: base(client)
		{
			this.ReadId = 0x08;
		}

		public PlayerBlockPlacement(ClientWrapper client, DataBuffer buffer)
			: base(client, buffer)
		{
			this.ReadId = 0x08;
		}

		public override void Read()
		{
			if (this.Buffer != null)
			{
				var position = this.Buffer.ReadPosition();

				if (position.Y > 256)
				{
					return;
				}

				var face = this.Buffer.ReadByte();

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

				var heldItem = this.Buffer.ReadUShort();
				if (heldItem <= ushort.MinValue || heldItem >= ushort.MaxValue)
				{
					return;
				}

				var itemCount = this.Buffer.ReadByte();
				var itemDamage = this.Buffer.ReadByte();
				var itemMeta = (byte)this.Buffer.ReadByte();

				var CursorX = this.Buffer.ReadByte(); // Unused
				var CursorY = this.Buffer.ReadByte(); // Unused
				var CursorZ = this.Buffer.ReadByte(); // Unused

				// 	if (position == new Vector3(-1, 256, -1))
				// {
				// 	ConsoleFunctions.WriteInfoLine("LOL, Update state <3");
				// 	}
				if (this.Client.Player.Level.GetBlock(position).Id == 0 || this.Client.Player.Level.GetBlock(position).IsReplacible)
				{
					if (this.Client.Player.Inventory.HasItem(heldItem) || this.Client.Player.Gamemode == Gamemode.Creative)
					{
						if (ItemFactory.GetItemById((short)heldItem).IsUsable)
						{
							ItemFactory.GetItemById((short)heldItem)
								.UseItem(this.Client.Player.Level, this.Client.Player, position, (BlockFace)face);
							return;
						}

						var b = BlockFactory.GetBlockById(heldItem);
						b.Coordinates = position;
						b.Metadata = itemMeta;
						this.Client.Player.Level.SetBlock(b, true, heldItem == 8 || heldItem == 10);

						if (this.Client.Player.Gamemode != Gamemode.Creative)
						{
							this.Client.Player.Inventory.RemoveItem((short)b.Id, itemMeta, 1);
						}
					}
					else
					{
						this.Client.Player.Inventory.SendToPlayer(); // Client not synced up, SYNC!
					}
				}
			}
		}
	}
}