namespace SharpMC.World.Standard.Settings
{
    public class NetherTweaking : WorldTweaking
    {
        public override int WaterLevel { get; set; } = 82;
        public override double BottomsFrequency { get; set; } = 2.2;
        public override double BottomOffset { get; set; } = 96.0;

        public double TopOffset { get; set; } = 32.0;
        public double TopMagnitude { get; set; } = 32.0;
        public double Topscale { get; set; } = 256.0;
        public double TopFrequency { get; set; } = 2.2;
        public double TopAmplitude { get; set; } = 0.5;
    }
}