using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public enum EquipmentSlot
	{
		Boots = 2,
		Leggings = 3,
		Chestplate = 4,
		Helmet = 5,
		Hand0 = 0,
		Hand1 = 1,
	}

	public class EntityEquipment : Package<EntityEquipment>
	{
		public int EntityId = 0;
		public ItemStack Item;
		public EquipmentSlot Slot = EquipmentSlot.Hand0;

		public EntityEquipment(ClientWrapper client) : base(client)
		{
			SendId = 0x04;
		}

		public EntityEquipment(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x04;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteVarInt(EntityId);
				Buffer.WriteVarInt((int)Slot);
				Buffer.WriteShort((short) Item.ItemId);
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