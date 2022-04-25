using System.Numerics;
using SharpMC.Blocks;
using SharpMC.World.Standard.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Structures
{
    internal class CactusStructure : StructureBase
    {
        public CactusStructure(IWorldContext context) : base(context)
        {
        }

        public override string Name => "Cactus";

        public override int Height => 3;

        public override (IBlock, Vector3)[] Blocks
        {
            get
            {
                return new (IBlock, Vector3)[]
                {
                    (Cactus, new Vector3(0, 0, 0)),
                    (Cactus, new Vector3(0, 1, 0)),
                    (Cactus, new Vector3(0, 2, 0))
                };
            }
        }
    }
}