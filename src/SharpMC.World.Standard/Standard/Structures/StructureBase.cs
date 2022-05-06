using System;
using System.Numerics;
using SharpMC.API.Blocks;
using SharpMC.API.Worlds;
using SharpMC.World.API.Structures;
using SharpMC.World.Standard.API;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Standard.Structures
{
    internal abstract class StructureBase : IStructure
    {
        protected readonly IWorldContext Context;

        protected StructureBase(IWorldContext context)
        {
            Context = context;
        }

        public virtual void Create(IChunkColumn chunk, int x, int y, int z)
        {
            if (chunk.GetBlock(x, y + Height, z).Equals(Air))
            {
                if (Blocks != null)
                    foreach (var (b, c) in Blocks)
                    {
                        var cx = (int) (x + c.X);
                        var cy = (int) (y + c.Y);
                        var cz = (int) (z + c.Z);
                        chunk.SetBlock(cx, cy, cz, b);
                    }
            }
        }

        public virtual string Name => null;

        public virtual int Height => 0;

        public virtual (IBlock, Vector3)[]? Blocks => null;

        protected void GenerateVanillaLeaves(IChunkColumn chunk, Vector3 location, int radius, IBlock block)
        {
            var radiusOffset = radius;
            for (var yOffset = -radius; yOffset <= radius; yOffset += 1)
            {
                var y = location.Y + yOffset;
                if (y > 256)
                    continue;
                GenerateVanillaCircle(chunk, new Vector3(location.X, y, location.Z), radiusOffset, block);
                if (yOffset != -radius && yOffset % 2 == 0)
                    radiusOffset--;
            }
        }

        protected void GenerateVanillaCircle(IChunkColumn chunk, Vector3 location, int radius,
            IBlock block, double corner = 0)
        {
            for (var I = -radius; I <= radius; I += 1)
            {
                for (var j = -radius; j <= radius; j += 1)
                {
                    var max = (int) Math.Sqrt(I * I + j * j);
                    if (max <= radius)
                    {
                        if (I.Equals(-radius) && j.Equals(-radius) || I.Equals(-radius) && j.Equals(radius) ||
                            I.Equals(radius) && j.Equals(-radius) || I.Equals(radius) && j.Equals(radius))
                        {
                            if (corner + radius * 0.2 < 0.4 || corner + radius * 0.2 > 0.7 || corner.Equals(0))
                                continue;
                        }
                        var x = location.X + I;
                        var z = location.Z + j;
                        if (chunk.GetBlock((int) x, (int) location.Y, (int) z).Equals(Air))
                        {
                            chunk.SetBlock((int) x, (int) location.Y, (int) z, block);
                        }
                    }
                }
            }
        }

        public static void GenerateColumn(IChunkColumn chunk, Vector3 location, int height, IBlock block)
        {
            for (var o = 0; o < height; o++)
            {
                var x = (int) location.X;
                var y = (int) location.Y + o;
                var z = (int) location.Z;
                chunk.SetBlock(x, y, z, block);
            }
        }

        protected void GenerateCircle(IChunkColumn chunk, Vector3 location, int radius, IBlock block)
        {
            for (var I = -radius; I <= radius; I += 1)
            {
                for (var j = -radius; j <= radius; j += 1)
                {
                    var max = (int) Math.Sqrt(I * I + j * j);
                    if (max <= radius)
                    {
                        var lX = location.X + I;
                        var lZ = location.Z + j;

                        if (lX < 0 || lX >= 16 || lZ < 0 || lZ >= 256)
                            continue;

                        var x = (int) lX;
                        var y = (int) location.Y;
                        var z = (int) lZ;
                        if (chunk.GetBlock(x, y, z).Equals(Air))
                        {
                            chunk.SetBlock(x, y, z, block);
                        }
                    }
                }
            }
        }
    }
}