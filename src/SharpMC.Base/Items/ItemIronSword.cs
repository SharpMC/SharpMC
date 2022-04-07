namespace SharpMC.Items
{
    internal class ItemIronSword : Item
    {
        public ItemIronSword() : base(267, 0)
        {
            ItemMaterial = ItemMaterial.Iron;
            ItemType = ItemType.Sword;
            MaxStackSize = 1;
        }
    }
}