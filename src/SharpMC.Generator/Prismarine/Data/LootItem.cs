namespace SharpMC.Generator.Prismarine.Data
{
    internal class LootItem
    {
        public string Item { get; set; }

        public double DropChance { get; set; }

        public int?[] StackSizeRange { get; set; }

        public bool? SilkTouch { get; set; }

        public bool? NoSilkTouch { get; set; }

        public int? BlockAge { get; set; }

        public bool? PlayerKill { get; set; }
    }
}