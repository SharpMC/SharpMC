namespace SharpMC.Items
{
    internal class ItemDiamondHelmet : Item
    {
        public ItemDiamondHelmet() : base(310, 0)
        {
            ItemType = ItemType.Helmet;
            ItemMaterial = ItemMaterial.Diamond;
            MaxStackSize = 1;
        }
    }
}