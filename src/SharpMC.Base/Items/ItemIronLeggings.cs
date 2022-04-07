namespace SharpMC.Items
{
	internal class ItemIronLeggings : Item
	{
		internal ItemIronLeggings() : base(308, 0)
		{
			ItemType = ItemType.Leggings;
			ItemMaterial = ItemMaterial.Iron;
			MaxStackSize = 1;
		}
	}
}