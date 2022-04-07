namespace SharpMC.Items
{
	internal class ItemDiamondChestplate : Item
	{
		public ItemDiamondChestplate() : base(311, 0)
		{
			ItemType = ItemType.Chestplate;
			ItemMaterial = ItemMaterial.Diamond;
			MaxStackSize = 1;
		}
	}
}