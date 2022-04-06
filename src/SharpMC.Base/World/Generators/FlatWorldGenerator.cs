using System.Collections.Concurrent;
using SharpMC.Util;

namespace SharpMC.World.Generators
{
	public class FlatWorldGenerator : IWorldGenerator
	{
		private ConcurrentDictionary<ChunkCoordinates, ChunkColumn> Chunks { get; }
			 
		public FlatWorldGenerator()
		{
			Chunks = new ConcurrentDictionary<ChunkCoordinates, ChunkColumn>();
		}

		public ChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates)
		{
			return Chunks.GetOrAdd(coordinates, CreateChunk);
		}

		public void Initialize()
		{
			
		}

		public PlayerLocation GetSpawnPoint()
		{
			return new PlayerLocation(0.5, 4f, 0.5f);
		}

		private static ChunkColumn CreateChunk(ChunkCoordinates chunkCoordinates)
		{
			ChunkColumn column = new ChunkColumn(chunkCoordinates);
			for (int y = 0; y < 4; y++)
			{
				for (int x = 0; x < ChunkColumn.Width; x++)
				{
					for (int z = 0; z < ChunkColumn.Depth; z++)
					{
						if (y == 0) column.SetBlockId(x, y, z, 7); //bedrock
						else if (y == 1 || y == 2) column.SetBlockId(x, y, z, 3); //Dirt
						else if (y == 3) column.SetBlockId(x, y, z, 2); //Grass
					}
				}
			}

			column.SetBlockId(0, 5, 0, 41);
			column.SetBlockId(0, 5, 1, 41);
			column.SetBlockId(0, 5, 2, 7);

			return column;
		}
	}
}
