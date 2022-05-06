using System;
using System.Numerics;
using SharpMC.API.Worlds;
using SharpMC.World.Standard.API;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Structures
{
    internal class PineTree : StructureBase
    {
        public PineTree(IWorldContext context) : base(context)
        {
        }

        public override string Name => nameof(PineTree);

        public override int Height => 10;

        public bool ValidLocation(Vector3 location)
        {
            var leafRadius = Context.Settings.LeafRadius;
            if (location.X - leafRadius < 0 || location.X + leafRadius >= 16 || location.Z - leafRadius < 0 ||
                location.Z + leafRadius >= 256)
                return false;
            return true;
        }

        public override void Create(IChunkColumn chunk, int x, int y, int z)
        {
            var location = new Vector3(x, y, z);
            if (!ValidLocation(new Vector3(x, y, z)))
                return;

            var leafRadius = Context.Settings.LeafRadius;

            var r = new Random();
            var height = r.Next(7, 8);
            GenerateColumn(chunk, location, height, SpruceWood);
            for (var yi = 1; yi < height; yi++)
            {
                if (yi % 2 == 0)
                {
                    GenerateVanillaCircle(chunk, location + new Vector3(0, yi + 1, 0), leafRadius - 1, SpruceLeaves);
                    continue;
                }
                GenerateVanillaCircle(chunk, location + new Vector3(0, yi + 1, 0),
                    leafRadius, SpruceLeaves);
            }

            GenerateTopper(chunk, location + new Vector3(0, height, 0), 0x1);
        }

        protected void GenerateTopper(IChunkColumn chunk, Vector3 location, byte type = 0x0)
        {
            var sectionRadius = 1;
            GenerateCircle(chunk, location, sectionRadius, SpruceLeaves);
            var top = location + new Vector3(0, 1, 0);
            var x = (int) location.X;
            var y = (int) location.Y + 1;
            var z = (int) location.Z;
            chunk.SetBlock(x, y, z, SpruceLeaves);
            if (type == 0x1 && y < 256)
                GenerateVanillaCircle(chunk, new Vector3(x, y, z),
                    sectionRadius, SpruceLeaves);
        }
    }
}