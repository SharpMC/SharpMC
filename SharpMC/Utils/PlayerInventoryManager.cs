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

namespace SharpMC.Utils
{
	using System;
	using System.Linq;

	using SharpMC.Entity;
	using SharpMC.Items;
	using SharpMC.Networking.Packages;

	public class PlayerInventoryManager
	{
		private readonly Player _player;

		private readonly ItemStack[] _slots = new ItemStack[45];

		private Item[] _craftingItems = new Item[4];

		private bool _isCrafting;

		public PlayerInventoryManager(Player player)
		{
			this._player = player;
			for (var i = 0; i <= 44; i++)
			{
				this._slots[i] = new ItemStack(-1, 0, 0);
			}

			this.SetSlot(5, 310, 0, 1); // Diamond helmet
			this.SetSlot(6, 311, 0, 1); // Diamond chestplate
			this.SetSlot(7, 312, 0, 1); // Diamond leggings
			this.SetSlot(8, 313, 0, 1); // Diamond boots

			this.SetSlot(36, 276, 0, 1); // Diamond sword
			this.SetSlot(37, 277, 0, 1); // Diamond shovel
			this.SetSlot(38, 278, 0, 1); // Diamond pickaxe
			this.SetSlot(39, 279, 0, 1); // Diamond axe

			this.SetSlot(43, 5, 0, 64);
			this.SetSlot(44, 332, 0, 64);

			this.SetSlot(41, 327, 0, 1);
			this.SetSlot(42, 326, 0, 1);
			this.SetSlot(40, 325, 0, 1);
		}

		public ItemStack ClickedItem { get; set; }

		public int CurrentSlot { get; set; }

		public void InventoryClosed()
		{
			this._isCrafting = false;
			foreach (var item in this._craftingItems)
			{
				if (item != null)
				{
					this.AddItem((short)item.Id, item.Metadata);
				}
			}

			this._craftingItems = new Item[4];
		}

		public bool HasItems(ItemStack[] items)
		{
			foreach (var item in items)
			{
				if (!this.HasItem(item.ItemId))
				{
					return false;
				}
			}

			return true;
		}

		public void SetSlot(int slot, short itemId, byte metadata, byte itemcount)
		{
			if (slot <= 44 && slot >= 5)
			{
				this._slots[slot] = new ItemStack(itemId, itemcount, metadata);
				if (this._player != null && this._player.IsSpawned)
				{
					new SetSlot(this._player.Wrapper)
						{
							WindowId = 0, 
							ItemId = itemId, 
							ItemCount = itemcount, 
							MetaData = metadata, 
							ItemDamage = 0, 
							Slot = (short)slot
						}.Write();
				}
			}
		}

		public bool AddItem(ItemStack item)
		{
			return this.AddItem(item.ItemId, item.MetaData, item.ItemCount);
		}

		public bool AddItem(short itemId, byte metadata, byte itemcount = 1)
		{
			for (var i = 9; i <= 44; i++)
			{
				if (this._slots[i].ItemId == itemId && this._slots[i].MetaData == metadata && this._slots[i].ItemCount < 64)
				{
					var oldslot = this._slots[i];
					if (oldslot.ItemCount + itemcount <= 64)
					{
						this.SetSlot(i, itemId, metadata, (byte)(oldslot.ItemCount + itemcount));
						return true;
					}

					this.SetSlot(i, itemId, metadata, 64);
					var remaining = (oldslot.ItemCount + itemcount) - 64;
					return this.AddItem(itemId, metadata, (byte)remaining);
				}
			}

			for (var i = 9; i <= 44; i++)
			{
				if (this._slots[i].ItemId == -1)
				{
					this.SetSlot(i, itemId, metadata, itemcount);
					return true;
				}
			}

			return false;
		}

		public ItemStack GetSlot(int slot)
		{
			if (slot <= 44 && slot >= 0)
			{
				return this._slots[slot];
			}

			throw new IndexOutOfRangeException("slot");
		}

		public void DropCurrentItem()
		{
			// Drop the current hold item
			var slottarget = 36 + this.CurrentSlot;
			var slot = this.GetSlot(slottarget);
			if (slot.ItemCount > 1)
			{
				this.SetSlot(slottarget, slot.ItemId, slot.MetaData, (byte)(slot.ItemCount - 1));
			}
			else
			{
				this.SetSlot(slottarget, -1, 0, 0);
			}

			if (slot.ItemId != -1)
			{
				new ItemEntity(this._player.Level, new ItemStack(slot.ItemId, 1, slot.MetaData))
					{
						KnownPosition =
							this._player.KnownPosition
					}
					.SpawnEntity();
			}
		}

		public void DropCurrentItemStack()
		{
			/*int slottarget = 36 + CurrentSlot;
			var slot = GetSlot(slottarget);
			if (slot.ItemId != -1)
			{
				for (int i = 0; i <= slot.ItemCount; i++)
				{
					new ItemEntity(_player.Level, new ItemStack(slot.ItemId, 1, slot.MetaData)) {KnownPosition = _player.KnownPosition}
						.SpawnEntity();
				}
				SetSlot(slottarget, -1, 0, 0);
			}*/
		}

		public bool HasItem(int itemId)
		{
			if (this._slots.Any(itemStack => itemStack.ItemId == itemId))
			{
				return true;
			}

			return false;
		}

		public bool RemoveItem(short itemId, short metaData, short count)
		{
			for (var index = 0; index <= 44; index++)
			{
				var itemStack = this._slots[index];
				if (itemStack.ItemId == itemId && itemStack.MetaData == metaData && itemStack.ItemCount >= count)
				{
					if ((itemStack.ItemCount - count) > 0)
					{
						this.SetSlot(index, itemStack.ItemId, itemStack.MetaData, (byte)(itemStack.ItemCount - count));
						return true;
					}

					this.SetSlot(index, -1, 0, 0);
					return true;
				}
			}

			return false;
		}

		public void SendToPlayer()
		{
			for (short i = 0; i <= 44; i++)
			{
				var value = this._slots[i];
				if (value.ItemId != -1)
				{
					new SetSlot(this._player.Wrapper)
						{
							WindowId = 0, 
							ItemId = value.ItemId, 
							ItemCount = value.ItemCount, 
							MetaData = value.MetaData, 
							Slot = i
						}.Write();
				}
			}
		}

		public byte[] GetBytes()
		{
			var buffer = new LocalDataBuffer(new byte[0]);
			for (var i = 0; i <= 44; i++)
			{
				var slot = this._slots[i];
				buffer.WriteInt(i); // Write the SlotID
				buffer.WriteShort(slot.ItemId); // Write the ItemID
				buffer.WriteByte(slot.MetaData);
				buffer.WriteByte(slot.ItemCount);
			}

			return buffer.ExportWriter;
		}

		public void Import(byte[] data)
		{
			var buffer = new LocalDataBuffer(data);

			for (var i = 0; i <= 44; i++)
			{
				var slotId = buffer.ReadInt();
				var itemId = buffer.ReadShort();
				var metaData = (byte)buffer.ReadByte();
				var itemCount = (byte)buffer.ReadByte();

				this._slots[slotId] = new ItemStack(itemId, itemCount, metaData);
			}
		}
	}
}