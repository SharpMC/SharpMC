using System.Collections.Generic;
using System;
using System.Net;
using System.Diagnostics;

namespace SharpMCRewrite
{
    public class Inventory
    {
        private SlotData[] _inventory = new SlotData[44];
        public short HeldItem = 0;

        public Inventory()
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                _inventory [i] = new SlotData ();
            }
        }

        public void SetSlot(int Slot, short ItemID, byte Amount = 0, short ItemDamage = 0)
        {
            if (Slot <= _inventory.Length)
            {
                _inventory [Slot].Item = ItemID;
                _inventory [Slot].ItemCount = Amount;
                _inventory [Slot].ItemDamage = ItemDamage;
            }
        }

        public short GetSlotItem(int Slot)
        {
            if (Slot <= _inventory.Length)
            {
                return _inventory [Slot].Item;
            }
            throw new IndexOutOfRangeException ("There is no such slot!");
        }

        public byte GetSlotItemCount(int Slot)
        {
            if (Slot <= _inventory.Length)
            {
                return _inventory [Slot].ItemCount;
            }
            throw new IndexOutOfRangeException ("There is no such slot!");
        }

        public short GetSlotItemDamage(int Slot)
        {
            if (Slot <= _inventory.Length)
            {
                return _inventory [Slot].ItemDamage;
            }
            throw new IndexOutOfRangeException ("There is no such slot!");
        }

        public byte[] toSlotArray()
        {
            MSGBuffer Writer = new MSGBuffer (new byte[0]);
            foreach (SlotData i in _inventory)
            {
                Writer.WriteShort (i.Item);
                if (i.Item != -1)
                {
                    Writer.WriteByte (i.ItemCount);
                    Writer.WriteShort (i.ItemCount);
                    Writer.WriteByte (0);
                }
            }
            return Writer.ExportWriter;
        }

        public void fromSlotArray(byte[] Data)
        {
            MSGBuffer reader = new MSGBuffer (Data);
            foreach (SlotData i in _inventory)
            {
                i.Item = reader.ReadShort ();
                if (i.Item != -1)
                {
                    i.ItemCount = (byte)reader.ReadByte ();
                    i.ItemDamage = reader.ReadShort ();
                    i.NBTData = (byte)reader.ReadByte ();
                }
            }
        }

        private class SlotData
        {
            public short Item { get; set; }
            public short ItemDamage { get; set; }
            public byte ItemCount { get; set; }
            public byte NBTData { get; set; }

            public SlotData()
            {
                Item = -1;
                ItemDamage = 0;
                ItemCount = 0;
                NBTData = 0;
            }
        }
    }
}

