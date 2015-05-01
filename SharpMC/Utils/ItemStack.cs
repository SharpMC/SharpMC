namespace SharpMC.Utils
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

		public short ItemId { get; set; }
		public byte ItemCount { get; set; }
		//public short ItemDamage { get; private set; }
		public byte MetaData { get; set; }
		public byte Nbt { get; private set; }
	}
}