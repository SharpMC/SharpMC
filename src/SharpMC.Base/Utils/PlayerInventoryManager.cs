using System;
using System.Linq;
using SharpMC.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Networking.Packages;
using SharpMC.Items;

namespace SharpMC.Core.Utils
{
    public class PlayerInventoryManager
    {
        private readonly Player _player;
        private readonly ItemStack[] _slots = new ItemStack[46];

        public PlayerInventoryManager(Player player)
        {
            _player = player;
            for (var i = 0; i <= 45; i++)
            {
                _slots[i] = new ItemStack(-1, 0, 0);
            }

            SetSlot(5, 310, 0, 1); //Diamond helmet
            SetSlot(6, 311, 0, 1); //Diamond chestplate
            SetSlot(7, 312, 0, 1); //Diamond leggings
            SetSlot(8, 313, 0, 1); //Diamond boots

            SetSlot(36, 276, 0, 1); //Diamond sword
            SetSlot(37, 277, 0, 1); //Diamond shovel
            SetSlot(38, 278, 0, 1); //Diamond pickaxe
            SetSlot(39, 279, 0, 1); //Diamond axe

            SetSlot(43, 5, 0, 64);
            SetSlot(44, 332, 0, 64);

            SetSlot(41, 327, 0, 1);
            SetSlot(42, 326, 0, 1);
            SetSlot(40, 325, 0, 1);

            UpdateHandItems();
        }

        public ItemStack ClickedItem { get; set; }
        public int CurrentSlot { get; set; }

        private Item Hand1 { get; set; }
        private Item Hand2 { get; set; }

        public void InventoryClosed()
        {
            if (ClickedItem != null)
            {
                AddItem(ClickedItem);
                ClickedItem = null;
            }
        }

        public void HeldItemChanged(int newSlot)
        {
            CurrentSlot = newSlot;
            UpdateHandItems();
        }

        private void UpdateHandItems()
        {
            var s = GetSlot(CurrentSlot + 36);
            Hand1 = s.ItemId > 255
                ? ItemFactory.GetItemById(s.ItemId, s.MetaData)
                : BlockFactory.GetBlockById((ushort) s.ItemId, s.MetaData);

            s = GetSlot(45);
            Hand2 = s.ItemId > 255
                ? ItemFactory.GetItemById(s.ItemId, s.MetaData)
                : BlockFactory.GetBlockById((ushort) s.ItemId, s.MetaData);
        }

        public Item GetItemInHand(int hand)
        {
            UpdateHandItems();
            switch (hand)
            {
                case 0:
                    return Hand1;
                case 1:
                    return Hand2;
                default:
                    throw new Exception("Invalid");
                    break;
            }
        }

        public void SwapHands()
        {
            UpdateHandItems();
            var mainhand = GetSlot(CurrentSlot + 36);
            var hand2 = GetSlot(45);
            SetSlot(CurrentSlot + 36, hand2.ItemId, hand2.MetaData, hand2.ItemCount);
            SetSlot(45, mainhand.ItemId, mainhand.MetaData, mainhand.ItemCount);
        }

        public bool HasItems(ItemStack[] items)
        {
            foreach (var item in items)
            {
                if (!HasItem(item.ItemId)) return false;
            }
            return true;
        }

        public void SetSlot(int slot, ushort itemId, byte metadata, byte itemcount)
        {
            SetSlot(slot, (short) itemId, metadata, itemcount);
        }

        public void SetSlot(int slot, short itemId, byte metadata, byte itemcount)
		{
			if (slot <= 45 && slot >= 5)
			{
				_slots[slot] = new ItemStack(itemId, itemcount, metadata);
				if (_player != null && _player.IsSpawned)
				{
					new SetSlot(_player.Wrapper)
					{
						WindowId = 0,
						ItemId = (ushort) itemId,
						ItemCount = itemcount,
						MetaData = metadata,
						ItemDamage = 0,
						Slot = (short) slot
					}.Write();
				}
			}
			UpdateHandItems();
		}

		public bool AddItem(ItemStack item)
		{
			return AddItem(item.ItemId, item.MetaData, item.ItemCount);
		}

		public bool AddItem(ushort itemId, byte metadata, byte itemcount = 1)
		{
			for (var i = 9; i <= 45; i++)
			{
				if (_slots[i].ItemId == itemId && _slots[i].MetaData == metadata && _slots[i].ItemCount < 64)
				{
					var oldslot = _slots[i];
					if (oldslot.ItemCount + itemcount <= 64)
					{
						SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemcount));
						return true;
					}
					SetSlot(i, itemId, metadata, 64);
					var remaining = oldslot.ItemCount + itemcount - 64;
					return AddItem(itemId, metadata, (byte) remaining);
				}
			}

			for (var i = 9; i <= 45; i++)
			{
				if (_slots[i].ItemId == -1)
				{
					SetSlot(i, itemId, metadata, itemcount);
					return true;
				}
			}
			return false;
		}

		public ItemStack GetSlot(int slot)
		{
			if (slot <= 45 && slot >= 0)
			{
				return _slots[slot];
			}
			throw new IndexOutOfRangeException("Invalid slot: " + slot);
		}

		public void DropCurrentItem()
		{
			//Drop the current hold item
			var slottarget = 36 + CurrentSlot;
			var slot = GetSlot(slottarget);
			if (slot.ItemCount > 1)
			{
				SetSlot(slottarget, slot.ItemId, slot.MetaData, (byte) (slot.ItemCount - 1));
			}
			else
			{
				SetSlot(slottarget, -1, 0, 0);
			}

			if (slot.ItemId != -1)
			{
				new ItemEntity(_player.Level, new ItemStack((short) slot.ItemId, 1, slot.MetaData)) {KnownPosition = _player.KnownPosition}
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
			if (_slots.Any(itemStack => itemStack.ItemId == itemId))
			{
				return true;
			}
			return false;
		}

		public bool RemoveItem(short itemId, short metaData, short count)
		{
			for (var index = 0; index <= 45; index++)
			{
				var itemStack = _slots[index];
				if (itemStack.ItemId == itemId && itemStack.MetaData == metaData && itemStack.ItemCount >= count)
				{
					if (itemStack.ItemCount - count > 0)
					{
						SetSlot(index, itemStack.ItemId, itemStack.MetaData, (byte) (itemStack.ItemCount - count));
						return true;
					}
					SetSlot(index, -1, 0, 0);
					return true;
				}
			}
			return false;
		}

		public void SendToPlayer()
		{
			for (short i = 0; i <= 45; i++)
			{
				var value = _slots[i];
				if (value.ItemId != -1)
				{
					new SetSlot(_player.Wrapper)
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
			var buffer = new DataBuffer(new byte[0]);
			for (var i = 0; i <= 45; i++)
			{
				var slot = _slots[i];
				buffer.WriteInt(i); //Write the SlotID
				buffer.WriteShort((short) slot.ItemId); //Write the ItemID
				buffer.WriteByte(slot.MetaData);
				buffer.WriteByte(slot.ItemCount);
			}
			return buffer.ExportWriter;
		}

		public void Import(byte[] data)
		{
			var buffer = new DataBuffer(data);

			for (var i = 0; i <= 45; i++)
			{
				var slotId = buffer.ReadInt();
				var itemId = buffer.ReadShort();
				var metaData = (byte)buffer.ReadByte();
				var itemCount = (byte)buffer.ReadByte();

				_slots[slotId] = new ItemStack(itemId, itemCount, metaData);
				UpdateHandItems();
			}
		}
	}
}