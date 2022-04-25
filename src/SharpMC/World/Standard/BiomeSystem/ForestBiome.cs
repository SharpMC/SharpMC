using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Decorators;
using SharpMC.World.Standard.Structures;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class ForestBiome : BiomeBase
    {
        public ForestBiome(IWorldContext context) : base(context)
        {
        }

        public override int Id => 1;

        public override BiomeIds MinecraftBiomeId => (BiomeIds) 4;

        public override IChunkDecorator[] Decorators
            => new IChunkDecorator[]
            {
                new GrassDecorator(Context),
                new ForestDecorator(Context)
            };

        public override IStructure[] TreeStructures
            => new IStructure[] {new BirchTree(Context), new OakTree(Context)};

        public override float Temperature => 0.7f;
    }
}