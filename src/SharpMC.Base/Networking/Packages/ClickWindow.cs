using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class ClickWindow : Package<ClickWindow>
	{
		public ClickWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0F;
		}

		public ClickWindow(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0F;
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

				if (itemId != -1)
				{
					itemCount = (byte)Buffer.ReadByte();
					itemDamage = Buffer.ReadShort();
					nbt = (byte)Buffer.ReadByte();
					if (nbt != 0)
					{
						//NBT Data found
					}
				}

				if (mode == 0)
				{
					if (itemId == -1)
					{
						var old = Client.Player.Inventory.GetSlot(Slot);
						var slot = Client.Player.Inventory.ClickedItem;
						if (old.ItemId == -1)
						{
							Client.Player.Inventory.SetSlot(Slot, slot.ItemId, slot.MetaData, slot.ItemCount);
							Client.Player.Inventory.ClickedItem = null;
						}
						else
						{
							if (old.ItemId == slot.ItemId && old.MetaData == slot.MetaData)
							{
								if (old.ItemCount < 64)
								{
									if (old.ItemCount + slot.ItemCount <= 64)
									{
										Client.Player.Inventory.SetSlot(Slot, slot.ItemId, slot.MetaData, (byte) (old.ItemCount + slot.ItemCount));
										Client.Player.Inventory.ClickedItem = null;
									}
									else
									{

									}
								}
							}
							else
							{
									
							}
						}
					}
					else
					{
						if (Client.Player.Inventory.GetSlot(Slot).ItemId == itemId) //Is the information true?
						{
							Client.Player.Inventory.SetSlot(Slot, -1, 0, 0);
							Client.Player.Inventory.ClickedItem = new ItemStack(itemId, itemCount, (byte)itemDamage);
						}
					}
				}


				/*if (Slot >= 1 && Slot <= 4) return; //We don't actually use that :)

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

				/*if (Slot == 0) //Crafting output.
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
				}*/
			}
		}
	}
}