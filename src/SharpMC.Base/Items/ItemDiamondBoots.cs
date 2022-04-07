namespace SharpMC.Items
{
	internal class ItemDiamondBoots : Item
	{
		public ItemDiamondBoots() : base(313, 0)
		{
			ItemType = ItemType.Boots;
			ItemMaterial = ItemMaterial.Diamond;
			MaxStackSize = 1;
		}
	}
}