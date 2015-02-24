using SharpMCRewrite.Blocks;

namespace SharpMCRewrite.Worlds.ExperimentalV2.Structures
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

		public void Create(ChunkColumn chunk, int x, int y, int z)
		{
			foreach (var b in Blocks)
			{
				chunk.SetBlock(x + b.Coordinates.X, y + b.Coordinates.Y, z + b.Coordinates.Z, b);
			}
		}
	}
}