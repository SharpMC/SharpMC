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
			if (id == 276) return new ItemDiamondSword();
			if (id == 310) return new ItemDiamondHelmet();
			if (id == 311) return new ItemDiamondChestplate();
			if (id == 312) return new ItemDiamondLeggings();
			if (id == 313) return new ItemDiamondBoots();
			if (id == 306) return new ItemIronHelmet();
			if (id == 307) return new ItemIronChestplate();
			if (id == 308) return new ItemIronLeggings();
			if (id == 309) return new ItemIronBoots();
			if (id == 267) return new ItemIronSword();
			return new Item((ushort) id, metadata);
		}
	}
}