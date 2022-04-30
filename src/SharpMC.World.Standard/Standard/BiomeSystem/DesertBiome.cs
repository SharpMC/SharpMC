using SharpMC.API.Blocks;
using SharpMC.API.Worlds;
using SharpMC.Data.Blocks;
using SharpMC.World.API.Structures;
using SharpMC.World.Standard.API;
using SharpMC.World.Standard.Structures;

namespace SharpMC.World.Standard.BiomeSystem
{
    internal class DesertBiome : BiomeBase
    {
        public DesertBiome(IWorldContext context) : base(context)
        {
        }

        public override int Id => 2;

        public override BiomeIds MinecraftBiomeId => (BiomeIds) 2;

        public override float Temperature => 2.0f;

        public override IBlock TopBlock => KnownBlocks.Sand;

        public override IBlock Filling => KnownBlocks.Sandstone;

        public override IStructure[] TreeStructures
            => new IStructure[] {new CactusStructure(Context)};

        public override int MaxTrees => 4;
    }
}