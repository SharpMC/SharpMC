namespace SharpMC.Generator.Prismarine.Data
{
    internal class Item
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int StackSize { get; set; }
        public int? MaxDurability { get; set; }
        public EnchantCategory[] EnchantCategories { get; set; }
        public string[] RepairWith { get; set; }
    }
}