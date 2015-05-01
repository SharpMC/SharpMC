namespace SharpMC.Items
{
	class ItemDiamondLeggings : Item
	{
		public ItemDiamondLeggings() : base(312, 0)
		{
			ItemType = ItemType.Leggings;
			ItemMaterial = ItemMaterial.Diamond;
			MaxStackSize = 1;
		}
	}
}
