namespace SharpMC.Generator.Prismarine.Data
{
    internal class Biome
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BiomeCategory Category { get; set; }
        public double Temperature { get; set; }
        public PrecipitationType Precipitation { get; set; }
        public double Depth { get; set; }
        public KnownDimension Dimension { get; set; }
        public string DisplayName { get; set; }
        public int Color { get; set; }
        public double Rainfall { get; set; }
    }
}