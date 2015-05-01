using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public class ClickWindow : Package<ClickWindow>
	{
		public ClickWindow(ClientWrapper client) : base(client)
		{
			ReadId = 0x0E;
		}

		public ClickWindow(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x0E;
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				byte WindowId = (byte)Buffer.ReadByte();
				short Slot = Buffer.ReadShort();
				byte Button = (byte) Buffer.ReadByte();
				short ActionNumber = Buffer.ReadShort();
				byte Mode = (byte) Buffer.ReadByte();

				short ItemId = Buffer.ReadShort();
				if (ItemId != -1)
				{
					byte ItemCount = (byte) Buffer.ReadByte();
					short ItemDamage = Buffer.ReadShort();
					byte Nbt = (byte) Buffer.ReadByte();
					if (Nbt != 0)
					{
						//NBT Data found
					}

					if (Client.Player.Inventory.GetSlot(Slot).ItemId == ItemId) //Is the information true?
					{
						Client.Player.Inventory.SetSlot(Slot, -1, 0 , 0);
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
