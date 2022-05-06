namespace SharpMC.Data.Foods
{
    public record Food
    {
        public int Id { get; init; }
        public string? DisplayName { get; init; }
        public string? Name { get; init; }
        public int StackSize { get; init; }
        public int FoodPoints { get; init; }
        public double Saturation { get; init; }
        public double EffectiveQuality { get; init; }
        public double SaturationRatio { get; init; }
    }
}