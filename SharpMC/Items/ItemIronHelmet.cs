namespace SharpMC.Items
{
	public class ItemIronHelmet : Item
	{
		public ItemIronHelmet() : base(306, 0)
		{
			ItemType = ItemType.Helmet;
			ItemMaterial = ItemMaterial.Iron;
			MaxStackSize = 1;
		}
	}
}
