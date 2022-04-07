using System;
using System.Numerics;
using SharpMC.Blocks;
using SharpMC.World;

namespace SharpMC.Core.Worlds.Standard.Structures
{
	public class Structure
	{
		public virtual string Name
		{
			get { return null; }
		}

		public virtual Block[] Blocks
		{
			get { return null; }
		}

		public virtual int Height
		{
			get { return 0; }
		}

		public virtual void Create(ChunkColumn chunk, int x, int y, int z)
		{
			if (chunk.GetBlock(x, y + Height, z) == 0)
			{
				foreach (var b in Blocks)
				{
					chunk.SetBlock(x + (int) b.Coordinates.X, y + (int) b.Coordinates.Y, z + (int) b.Coordinates.Z, b);
				}
			}
		}

		protected void GenerateVanillaLeaves(ChunkColumn chunk, Vector3 location, int radius, Block block)
		{
			var radiusOffset = radius;
			for (var yOffset = -radius; yOffset <= radius; yOffset = (yOffset + 1))
			{
				var y = location.Y + yOffset;
				if (y > 256)
					continue;
				GenerateVanillaCircle(chunk, new Vector3(location.X, y, location.Z), radiusOffset, block);
				if (yOffset != -radius && yOffset%2 == 0)
					radiusOffset--;
			}
		}

		protected void GenerateVanillaCircle(ChunkColumn chunk, Vector3 location, int radius, Block block, double corner = 0)
		{
			for (var I = -radius; I <= radius; I = (I + 1))
			{
				for (var j = -radius; j <= radius; j = (j + 1))
				{
					var max = (int) Math.Sqrt((I*I) + (j*j));
					if (max <= radius)
					{
						if (I.Equals(-radius) && j.Equals(-radius) || I.Equals(-radius) && j.Equals(radius) ||
						    I.Equals(radius) && j.Equals(-radius) || I.Equals(radius) && j.Equals(radius))
						{
							if (corner + radius*0.2 < 0.4 || corner + radius*0.2 > 0.7 || corner.Equals(0))
								continue;
						}
						var x = location.X + I;
						var z = location.Z + j;
						if (chunk.GetBlock((int) x, (int) location.Y, (int) z).Equals(0))
						{
							chunk.SetBlock((int) x, (int) location.Y, (int) z, block);
						}
					}
				}
			}
		}

		public static void GenerateColumn(ChunkColumn chunk, Vector3 location, int height, Block block)
		{
			for (var o = 0; o < height; o++)
			{
				var x = (int) location.X;
				var y = (int) location.Y + o;
				var z = (int) location.Z;
				chunk.SetBlock(x, y, z, block);
			}
		}

		protected void GenerateCircle(ChunkColumn chunk, Vector3 location, int radius, Block block)
		{
			for (var I = -radius; I <= radius; I = (I + 1))
			{
				for (var j = -radius; j <= radius; j = (j + 1))
				{
					var max = (int) Math.Sqrt((I*I) + (j*j));
					if (max <= radius)
					{
						var X = location.X + I;
						var Z = location.Z + j;

						if (X < 0 || X >= 16 || Z < 0 || Z >= 256)
							continue;

						var x = (int) X;
						var y = (int) location.Y;
						var z = (int) Z;
						if (chunk.GetBlock(x, y, z).Equals(0))
						{
							chunk.SetBlock(x, y, z, block);
						}
					}
				}
			}
		}
	}
}