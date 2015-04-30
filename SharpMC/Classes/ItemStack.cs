namespace SharpMC.Classes
{
	public class ItemStack
	{
		public ItemStack(short itemId, byte itemCount, byte metadata)
		{
			ItemId = itemId;
			ItemCount = itemCount;
		//	ItemDamage = itemDamage;
			Nbt = 0;
			MetaData = metadata;
		}

		public short ItemId { get; private set; }
		public byte ItemCount { get; private set; }
		//public short ItemDamage { get; private set; }
		public byte MetaData { get; private set; }
		public byte Nbt { get; private set; }
	}
}