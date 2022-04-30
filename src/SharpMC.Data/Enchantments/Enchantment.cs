namespace SharpMC.Data.Enchantments
{
    public record Enchantment
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? DisplayName { get; init; }
        public int MaxLevel { get; init; }
        public bool TreasureOnly { get; init; }
        public bool Curse { get; init; }
        public int Weight { get; init; }
        public bool Tradeable { get; init; }
        public bool Discoverable { get; init; }
        public EnchantCategory Category { get; init; }
        public string[]? Exclude { get; init; }
    }
}