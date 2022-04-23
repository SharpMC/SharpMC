namespace SharpMC.Enchantments
{
    public class Enchantment
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int MaxLevel { get; set; }
        public bool TreasureOnly { get; set; }
        public bool Curse { get; set; }
        public int Weight { get; set; }
        public bool Tradeable { get; set; }
        public bool Discoverable { get; set; }
        public string[] Exclude { get; set; }
        public EnchantCategory Category { get; set; }
    }
}