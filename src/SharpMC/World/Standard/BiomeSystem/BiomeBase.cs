using SharpMC.Blocks;
using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Decorators;
using SharpMC.World.Standard.Structures;
using SharpMC.Worlds.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal abstract class BiomeBase : IBiomeBase
    {
        protected readonly IWorldContext Context;

        protected BiomeBase(IWorldContext context)
        {
            Context = context;
        }

        public virtual double BaseHeight => 52.0;

        public virtual int Id => 0;

        public virtual BiomeIds MinecraftBiomeId => 0;

        public virtual int MaxTrees => 10;

        public virtual int MinTrees => 0;

        public virtual IStructure[] TreeStructures
            => new IStructure[] {new OakTree(Context)};

        public virtual IChunkDecorator[] Decorators
            => new IChunkDecorator[] {new TreeDecorator(Context)};

        public virtual float Temperature => 0.0f;

        public virtual IBlock TopBlock => Grass;

        public virtual IBlock Filling => Dirt;
    }
}