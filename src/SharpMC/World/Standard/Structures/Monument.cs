using System.Numerics;
using SharpMC.Blocks;
using SharpMC.World.Standard.API;
using static SharpMC.Blocks.KnownBlocks;

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
                    (GoldBlock, new Vector3(0, 0, 0)),
                    (BrickWall, new Vector3(0, 0, 0)),
                    (BrickWall, new Vector3(1, 0, 0)),
                    (BrickWall, new Vector3(2, 0, 0)),
                    (BrickWall, new Vector3(3, 0, 0)),
                    (BrickWall, new Vector3(0, 0, 1)),
                    (BrickWall, new Vector3(1, 0, 1)),
                    (BrickWall, new Vector3(2, 0, 1)),
                    (BrickWall, new Vector3(3, 0, 1)),
                    (BrickWall, new Vector3(0, 0, 2)),
                    (BrickWall, new Vector3(1, 0, 2)),
                    (BrickWall, new Vector3(2, 0, 2)),
                    (BrickWall, new Vector3(3, 0, 2)),
                    (BrickWall, new Vector3(0, 0, 3)),
                    (BrickWall, new Vector3(1, 0, 3)),
                    (BrickWall, new Vector3(2, 0, 3)),
                    (BrickWall, new Vector3(3, 0, 3))
                };
            }
        }
    }
}