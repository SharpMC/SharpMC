using System;
using System.Collections.Generic;
using SharpMC.Classes;
using SharpMC.Networking.Packages;

namespace SharpMC
{
	public class PlayerInventoryManager
	{
		private readonly Player _player;
		private readonly List<Slot> _slots = new List<Slot>();

		public PlayerInventoryManager(Player player)
		{
			_player = player;
			for (var i = 0; i < 44; i++)
			{
				_slots.Add(new Slot(1, 64, 1, 0));
			}
		}

		public void SetSlot(int slot, short itemId, byte metadata, byte itemcount = 1)
		{
			if (slot <= 44 && slot > 0)
			{
				//_slots[slot] = new Tuple<short, byte, byte>(itemId, itemcount, metadata);
				_slots[slot] = new Slot(itemId, itemcount, 0, metadata);
			}
		}

		public bool SetSlot(short itemId, byte metadata, byte itemcount = 1)
		{
			for (var i = 0; i < _slots.Count; i++)
			{
				if (_slots[i].ItemId == -1)
				{
					//_slots[i] = new Tuple<short, byte, byte>(itemId, itemcount, metadata);
					_slots[i] = new Slot(itemId, itemcount, 0, metadata);
					return true;
				}
			}
			return false;
		}

		public Slot GetSlot(int slot)
		{
			if (slot <= 44 && slot > 0)
			{
				return _slots[slot];
			}
			throw new IndexOutOfRangeException("slot");
		}

		public void SendToPlayer()
		{
			for (var i = 0; i < _slots.Count; i++)
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
						ItemDamage = value.ItemDamage,
						Slot = (short) i
					}.Write();
				}
			}
			//	new WindowItems(_player.Wrapper)
			//	{
			//		WindowId = 0,
			//		Slots = _slots.ToArray()
			//	}.Write();
		}
	}
}