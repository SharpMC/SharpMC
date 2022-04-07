namespace SharpMC.Items
{
	internal class ItemDiamondSword : Item
	{
		internal ItemDiamondSword() : base(276, 0)
		{
			ItemType = ItemType.Sword;
			ItemMaterial = ItemMaterial.Diamond;
			MaxStackSize = 1;
		}
	}
}