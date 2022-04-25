using SharpMC.World.Standard.API;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class OceanBiome : BiomeBase
    {
        public OceanBiome(IWorldContext context) : base(context)
        {
        }

        public override double BaseHeight => 32;

        public override float Temperature => 0.5f;

        public override BiomeIds MinecraftBiomeId => 0;
    }
}