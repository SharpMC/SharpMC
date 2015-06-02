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

using SharpMC.Blocks;
using SharpMC.Items;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
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
				var WindowId = (byte) Buffer.ReadByte();
				var Slot = Buffer.ReadShort();
				var Button = (byte) Buffer.ReadByte();
				var ActionNumber = Buffer.ReadShort();
				var Mode = (byte) Buffer.ReadByte();

				var ItemId = Buffer.ReadShort();

				byte ItemCount = 1;
				short ItemDamage = 0;
				byte Nbt = 0;

				if (Slot >= 1 && Slot <= 4) return; //We don't actually use that :)

				if (ItemId != -1)
				{
					ItemCount = (byte) Buffer.ReadByte();
					ItemDamage = Buffer.ReadShort();
					Nbt = (byte) Buffer.ReadByte();
					if (Nbt != 0)
					{
						//NBT Data found
					}
				}

				if (Slot == 0) //Crafting output.
				{
					var item = ItemFactory.GetItemById(ItemId);
					if (item.CraftingItems != null) //Valid
					{
						if (Client.Player.Inventory.HasItems(item.CraftingItems))
						{
							Client.Player.Inventory.AddItem(ItemId, (byte)ItemDamage, ItemCount);
						}
					}
					else
					{
						var block = BlockFactory.GetBlockById((ushort)ItemId);
						if (block.CraftingItems != null) //valid
						{
							if (Client.Player.Inventory.HasItems(block.CraftingItems))
							{
								Client.Player.Inventory.AddItem(ItemId, (byte)ItemDamage, ItemCount);
							}
						}
					}
					return;
				}

				if (ItemId != -1)
				{
					
					if (Client.Player.Inventory.GetSlot(Slot).ItemId == ItemId) //Is the information true?
					{
						Client.Player.Inventory.SetSlot(Slot, -1, 0, 0);
						Client.Player.Inventory.ClickedItem = new ItemStack(ItemId, ItemCount, (byte) ItemDamage);
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