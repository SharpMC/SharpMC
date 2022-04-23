namespace SharpMC.Biomes
{
    public class Biome
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public BiomeCategory Category { get; set; }
        public double Temperature { get; set; }
        public double Depth { get; set; }
        public int Color { get; set; }
        public double Rainfall { get; set; }
        public BiomePrecipitation Precipitation { get; set; }
        public BiomeDim Dimension { get; set; }
    }
}