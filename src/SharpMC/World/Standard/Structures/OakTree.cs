using System;
using System.Numerics;
using SharpMC.Blocks;
using SharpMC.World.Standard.API;
using static SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Structures
{
    internal class OakTree : StructureBase
    {
        public OakTree(IWorldContext context) : base(context)
        {
        }

        public override string Name => nameof(OakTree);

        public override int Height => 10;

        public override void Create(IChunkColumn chunk, int x, int y, int z)
        {
            if (!Context.Settings.UseVanillaTrees)
            {
                base.Create(chunk, x, y, z);
                return;
            }

            var location = new Vector3(x, y, z);
            if (!ValidLocation(new Vector3(x, y, z)))
                return;

            var leafRadius = Context.Settings.LeafRadius;
            var random = new Random();
            var height = random.Next(4, 5);
            GenerateColumn(chunk, location, height, OakWood);
            var leafLocation = location + new Vector3(0, height, 0);
            GenerateVanillaLeaves(chunk, leafLocation, leafRadius, OakLeaves);
        }

        public bool ValidLocation(Vector3 location)
        {
            var leafRadius = Context.Settings.LeafRadius;
            if (location.X - leafRadius < 0 || location.X + leafRadius >= 16 || location.Z - leafRadius < 0 ||
                location.Z + leafRadius >= 256)
                return false;
            return true;
        }

        public override (IBlock, Vector3)[] Blocks
        {
            get
            {
                return new (IBlock, Vector3)[]
                {
                    (OakWood , new Vector3(0, 0, 0)),
                    (OakWood , new Vector3(0, 1, 0)),
                    (OakWood, new Vector3(0, 2, 0)),
                    (OakWood , new Vector3(0, 3, 0)),
                    (OakWood, new Vector3(0, 4, 0)),
                    (OakWood, new Vector3(0, 5, 0)),
                    (OakLeaves, new Vector3(-1, 4, 1)),
                    (OakLeaves, new Vector3(1, 4, -1)),
                    (OakLeaves, new Vector3(-1, 4, -1)),
                    (OakLeaves, new Vector3(1, 4, 1)),
                    (OakLeaves, new Vector3(-1, 4, 0)),
                    (OakLeaves , new Vector3(1, 4, 0)),
                    (OakLeaves, new Vector3(0, 4, -1)),
                    (OakLeaves, new Vector3(0, 4, 1)),
                    (OakLeaves, new Vector3(-1, 5, 0)),
                    (OakLeaves, new Vector3(1, 5, 0)),
                    (OakLeaves, new Vector3(0, 5, -1)),
                    (OakLeaves, new Vector3(0, 5, 1)),
                    (OakLeaves, new Vector3(0, 6, 0))
                };
            }
        }
    }
}