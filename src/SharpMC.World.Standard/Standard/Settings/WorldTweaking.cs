using SharpMC.World.Standard.API;

namespace SharpMC.World.Standard.Settings
{
    public class WorldTweaking : IWorldSettings
    {
        /// <summary>
        /// Changes the offset from the bottom ground
        /// </summary>
        public double OverhangOffset { get; set; } = 32.0;

        /// <summary>
        /// Changes the offset from y level 0
        /// </summary>
        public virtual double BottomOffset { get; set; } = 42.0;

        /// <summary>
        /// Changes the scale of the overhang
        /// </summary>
        public double OverhangScale { get; set; } = 128.0;

        /// <summary>
        /// Changes the scale of the ground
        /// </summary>
        public double Groundscale { get; set; } = 256.0;

        public string Seed { get; set; } = "SharpMC";

        public double OverhangsMagnitude { get; set; } = 16.0;
        public double BottomsMagnitude { get; set; } = 32.0;
        public double Threshold { get; set; } = 0.1;
        public virtual double BottomsFrequency { get; set; } = 0.5;
        public double BottomsAmplitude { get; set; } = 0.5;
        public double OverhangFrequency { get; set; } = 0.5;
        public double OverhangAmplitude { get; set; } = 0.5;
        public bool EnableOverhang { get; set; } = true;
        public virtual int WaterLevel { get; set; } = 72;

        public int LeafRadius { get; set; } = 2;
        public bool UseVanillaTrees { get; set; } = false;
    }
}