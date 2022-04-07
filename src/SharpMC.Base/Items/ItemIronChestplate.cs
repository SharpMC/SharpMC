namespace SharpMC.Items
{
    internal class ItemIronChestplate : Item
    {
        internal ItemIronChestplate() : base(307, 0)
        {
            ItemType = ItemType.Chestplate;
            ItemMaterial = ItemMaterial.Iron;
            MaxStackSize = 1;
        }
    }
}