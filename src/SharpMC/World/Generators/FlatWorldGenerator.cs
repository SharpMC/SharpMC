using System.Collections.Concurrent;
using SharpMC.Blocks;
using SharpMC.Players;
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
            var column = new ChunkColumn(chunkCoordinates);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < ChunkColumn.Width; x++)
                {
                    for (var z = 0; z < ChunkColumn.Depth; z++)
                    {
                        if (y == 0) column.SetBlockId(x, y, z, BlockIds.Bedrock);
                        else if (y == 1 || y == 2) column.SetBlockId(x, y, z, BlockIds.Dirt);
                        else if (y == 3) column.SetBlockId(x, y, z, BlockIds.Grass);
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