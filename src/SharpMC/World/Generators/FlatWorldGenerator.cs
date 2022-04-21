using System.Collections.Concurrent;
using SharpMC.Players;
using SharpMC.Util;
using BlockIds = SharpMC.Blocks.KnownBlocks;

namespace SharpMC.World.Generators
{
    public class FlatWorldGenerator : IWorldGenerator
    {
        private ConcurrentDictionary<ChunkCoordinates, IChunkColumn> Chunks { get; }

        public FlatWorldGenerator()
        {
            Chunks = new ConcurrentDictionary<ChunkCoordinates, IChunkColumn>();
        }

        public IChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates)
        {
            return Chunks.GetOrAdd(coordinates, CreateChunk);
        }

        public PlayerLocation GetSpawnPoint()
        {
            return new PlayerLocation(0.5, 4f, 0.5f);
        }

        private static IChunkColumn CreateChunk(ChunkCoordinates _)
        {
            IChunkColumn column = new ChunkColumn();
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < column.Width; x++)
                {
                    for (var z = 0; z < column.Depth; z++)
                    {
                        if (y == 0) column.SetBlock(x, y, z, BlockIds.Sand);
                        else if (y == 1 || y == 2) column.SetBlock(x, y, z, BlockIds.Dirt);
                        else if (y == 3) column.SetBlock(x, y, z, BlockIds.Grass);
                    }
                }
            }
            for (var i = 0; i < 6; i++)
            {
                column.SetBlock(0, 5 + i, 0, BlockIds.DiamondBlock);
            }
            column.SetBlock(0, 5, 0, BlockIds.GoldBlock);
            column.SetBlock(0, 5, 1, BlockIds.GoldBlock);
            column.SetBlock(0, 5, 2, BlockIds.Bedrock);
            return column;
        }
    }
}