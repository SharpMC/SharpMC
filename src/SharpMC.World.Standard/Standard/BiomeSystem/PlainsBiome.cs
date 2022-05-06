using SharpMC.API.Worlds;
using SharpMC.World.API.Chunks;
using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Decorators;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class PlainsBiome : BiomeBase
    {
        public PlainsBiome(IWorldContext context) : base(context)
        {
        }

        public override int Id => 0;

        public override BiomeIds MinecraftBiomeId => (BiomeIds) 1;

        public override IChunkDecorator[] Decorators
            => new IChunkDecorator[]
            {
                new GrassDecorator(Context)
            };

        public override float Temperature => 0.8f;

        public override int MaxTrees => 0;
    }
}