using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
	public enum EquipmentSlot
	{
		Held = 0,
		Boots = 1,
		Leggings = 2,
		Chestplate = 3,
		Helmet = 4
	}

	public class EntityEquipment : Package<EntityEquipment>
	{
		public int EntityId = 0;
		public EquipmentSlot Slot = EquipmentSlot.Held;
		public ItemStack Item;

		public EntityEquipment(ClientWrapper client) : base(client)
		{
			SendId = 0x04;
		}

		public EntityEquipment(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x04;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteShort((short)Slot);
				Buffer.WriteShort(Item.ItemId);
				if (Item.ItemId != -1)
				{
					Buffer.WriteByte(1);
					Buffer.WriteShort(Item.MetaData);
					Buffer.WriteByte(0);
				}
				Buffer.FlushData();
			}
		}
	}
}
