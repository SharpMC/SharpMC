using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Decorators;
using SharpMC.World.Standard.Structures;
using SharpMC.Worlds.API;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class FlowerForestBiome : BiomeBase
    {
        public FlowerForestBiome(IWorldContext context) : base(context)
        {
        }

        public override int Id => 4;

        public override BiomeIds MinecraftBiomeId => (BiomeIds) 132;

        public override IChunkDecorator[] Decorators
            => new IChunkDecorator[]
            {
                new GrassDecorator(Context),
                new TreeDecorator(Context),
                new FlowerDecorator(Context)
            };

        public override IStructure[] TreeStructures
            => new IStructure[] {new BirchTree(Context), new OakTree(Context)};

        public override float Temperature => 0.7f;

        public override int MaxTrees => 30;

        public override int MinTrees => 20;
    }
}