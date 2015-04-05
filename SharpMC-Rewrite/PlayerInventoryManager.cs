using System;
using System.Collections.Generic;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Networking.Packages;

namespace SharpMCRewrite
{
	public class PlayerInventoryManager
	{
		private readonly Player _player;
		private readonly List<Tuple<short, byte, byte>> _slots = new List<Tuple<short, byte, byte>>();

		public PlayerInventoryManager(Player player)
		{
			_player = player;
			for (var i = 0; i <= 44; i++)
			{
				_slots.Add(new Tuple<short, byte, byte>(1, 1, 0));
			}
		}

		public void SetSlot(int slot, short itemId, byte metadata, byte itemcount = 1)
		{
			if (slot <= 44 && slot > 0)
			{
				_slots[slot] = new Tuple<short, byte, byte>(itemId, itemcount, metadata);
			}
		}

		public bool SetSlot(short itemId, byte metadata, byte itemcount = 1)
		{
			for (var i = 0; i < _slots.Count; i++)
			{
				if (_slots[i].Item1 == 0)
				{
					_slots[i] = new Tuple<short, byte, byte>(itemId, itemcount, metadata);
					return true;
				}
			}
			return false;
		}

		public Tuple<short, byte, byte> GetSlot(int slot)
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
				new SetSlot(_player.Wrapper)
				{
					WindowId = 0,
					ItemId = value.Item1,
					ItemCount = value.Item2,
					MetaData = value.Item3,
					ItemDamage = 1,
					Slot = (short) i
				}.Write();
			}
		}
	}
}