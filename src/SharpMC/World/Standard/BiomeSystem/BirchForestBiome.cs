using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Decorators;
using SharpMC.World.Standard.Structures;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class BirchForestBiome : BiomeBase
    {
        public BirchForestBiome(IWorldContext context) : base(context)
        {
        }

        public override int Id => 3;

        public override BiomeIds MinecraftBiomeId => (BiomeIds) 27;

        public override IChunkDecorator[] Decorators
            => new IChunkDecorator[]
            {
                new GrassDecorator(Context),
                new ForestDecorator(Context)
            };

        public override IStructure[] TreeStructures
            => new IStructure[] {new BirchTree(Context)};

        public override float Temperature => 0.7f;
    }
}