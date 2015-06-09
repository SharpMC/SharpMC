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
using SharpMC.Core.Items;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class ClickWindow : Package<ClickWindow>
	{
		public ClickWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0E;
		}

		public ClickWindow(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0E;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var windowId = (byte) Buffer.ReadByte();
				var Slot = Buffer.ReadShort();
				var button = (byte) Buffer.ReadByte();
				var actionNumber = Buffer.ReadShort();
				var mode = (byte) Buffer.ReadByte();

				var itemId = Buffer.ReadShort();

				byte itemCount = 1;
				short itemDamage = 0;
				byte nbt = 0;

				if (Slot >= 1 && Slot <= 4) return; //We don't actually use that :)

				if (itemId != -1)
				{
					itemCount = (byte) Buffer.ReadByte();
					itemDamage = Buffer.ReadShort();
					nbt = (byte) Buffer.ReadByte();
					if (nbt != 0)
					{
						//NBT Data found
					}
				}

				if (Slot == 0) //Crafting output.
				{
					var item = ItemFactory.GetItemById(itemId);
					if (item.CraftingItems != null) //Valid
					{
						if (Client.Player.Inventory.HasItems(item.CraftingItems))
						{
							Client.Player.Inventory.AddItem(itemId, (byte)itemDamage, itemCount);
						}
					}
					else
					{
						var block = BlockFactory.GetBlockById((ushort)itemId);
						if (block.CraftingItems != null) //valid
						{
							if (Client.Player.Inventory.HasItems(block.CraftingItems))
							{
								Client.Player.Inventory.AddItem(itemId, (byte)itemDamage, itemCount);
							}
						}
					}
					return;
				}

				if (itemId != -1)
				{
					
					if (Client.Player.Inventory.GetSlot(Slot).ItemId == itemId) //Is the information true?
					{
						Client.Player.Inventory.SetSlot(Slot, -1, 0, 0);
						Client.Player.Inventory.ClickedItem = new ItemStack(itemId, itemCount, (byte) itemDamage);
					}
				}
				else
				{
					var slot = Client.Player.Inventory.ClickedItem;
					Client.Player.Inventory.SetSlot(Slot, slot.ItemId, slot.MetaData, slot.ItemCount);
				}
			}
		}
	}
}