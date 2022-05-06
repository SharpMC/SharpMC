using System.Numerics;
using SharpMC.API.Blocks;
using SharpMC.World.Standard.API;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Structures
{
    internal class Monument : StructureBase
    {
        public Monument(IWorldContext context) : base(context)
        {
        }

        public override string Name => nameof(Monument);

        public override (IBlock,Vector3)[] Blocks
        {
            get
            {
                return new (IBlock, Vector3)[]
                {
                    (IronBlock, new Vector3(0, 0, 0)),
                    (SandstoneSlab, new Vector3(0, 0, 0)),
                    (SandstoneSlab, new Vector3(1, 0, 0)),
                    (SandstoneSlab, new Vector3(2, 0, 0)),
                    (SandstoneSlab, new Vector3(3, 0, 0)),
                    (SandstoneSlab, new Vector3(0, 0, 1)),
                    (SandstoneSlab, new Vector3(1, 0, 1)),
                    (SandstoneSlab, new Vector3(2, 0, 1)),
                    (SandstoneSlab, new Vector3(3, 0, 1)),
                    (SandstoneSlab, new Vector3(0, 0, 2)),
                    (SandstoneSlab, new Vector3(1, 0, 2)),
                    (SandstoneSlab, new Vector3(2, 0, 2)),
                    (SandstoneSlab, new Vector3(3, 0, 2)),
                    (SandstoneSlab, new Vector3(0, 0, 3)),
                    (SandstoneSlab, new Vector3(1, 0, 3)),
                    (SandstoneSlab, new Vector3(2, 0, 3)),
                    (SandstoneSlab, new Vector3(3, 0, 3))
                };
            }
        }
    }
}