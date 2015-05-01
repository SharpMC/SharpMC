namespace SharpMC.Items
{
	internal class ItemFactory
	{
		public static Item GetItemById(short id)
		{
			return GetItemById(id,0);
		}

		public static Item GetItemById(short id, byte metadata)
		{
			if (id == 259) return new ItemFlintAndSteel();
			if (id == 263) return new ItemCoal();
			return new Item((ushort) id, metadata);
		}
	}
}