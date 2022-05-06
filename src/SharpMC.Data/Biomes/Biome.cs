namespace SharpMC.Data.Biomes
{
    public record Biome
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? DisplayName { get; init; }
        public BiomeCategory Category { get; init; }
        public double Temperature { get; init; }
        public BiomePrecipitation Precipitation { get; init; }
        public double Depth { get; init; }
        public int Color { get; init; }
        public double Rainfall { get; init; }
        public BiomeDim Dimension { get; init; }
    }
}