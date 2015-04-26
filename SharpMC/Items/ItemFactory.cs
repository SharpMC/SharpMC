namespace SharpMC.Items
{
	internal class ItemFactory
	{
		public static Item GetItemById(ushort id, byte metadata)
		{
			if (id == 259) return new ItemFlintAndSteel(metadata);
			return new Item(id, metadata);
		}
	}
}