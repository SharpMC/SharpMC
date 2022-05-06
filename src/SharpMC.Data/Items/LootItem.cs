namespace SharpMC.Data.Items
{
    public record LootItem
    {
        public string? Item { get; init; }
        public double DropChance { get; init; }
        public int[]? StackSizeRange { get; init; }
        public int? BlockAge { get; init; }
        public bool? SilkTouch { get; init; }
        public bool? NoSilkTouch { get; init; }
        public bool? PlayerKill { get; init; }
    }
}