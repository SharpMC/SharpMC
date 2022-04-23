namespace SharpMC.Foods
{
    public class Food
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int StackSize { get; set; }
        public int FoodPoints { get; set; }
        public double Saturation { get; set; }
        public double EffectiveQuality { get; set; }
        public double SaturationRatio { get; set; }
    }
}