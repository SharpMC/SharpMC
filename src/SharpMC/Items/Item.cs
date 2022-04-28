namespace SharpMC.Items
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int StackSize { get; set; }

        public ItemType? ItemType { get; set; }
        public ItemMaterial? ItemMaterial { get; set; }
    }
}