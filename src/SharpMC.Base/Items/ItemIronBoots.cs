namespace SharpMC.Items
{
	internal class ItemIronBoots : Item
	{
		internal ItemIronBoots() : base(309, 0)
		{
			ItemType = ItemType.Boots;
			ItemMaterial = ItemMaterial.Iron;
			MaxStackSize = 1;
		}
	}
}