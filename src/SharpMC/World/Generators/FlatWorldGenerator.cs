using System.Collections.Concurrent;
using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Structures;
using static SharpMC.Blocks.KnownBlocks;

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

        public PlayerLocation SpawnPoint
            => new PlayerLocation(0.5, 4f, 0.5f);

        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
            => CreateChunk(chunk, pos);

        private static IChunkColumn CreateChunk(ChunkCoordinates _)
        {
            IChunkColumn column = new ChunkColumn();
            CreateChunk(column, _);
            return column;
        }

        private static void CreateChunk(IChunkColumn column, ChunkCoordinates _)
        {
            column.SetBlock(0, 0, 0, Air);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < column.Width; x++)
                {
                    for (var z = 0; z < column.Depth; z++)
                    {
                        if (y == 0) column.SetBlock(x, y, z, Bedrock);
                        else if (y == 1 || y == 2) column.SetBlock(x, y, z, Dirt);
                        else if (y == 3) column.SetBlock(x, y, z, Grass);
                    }
                }
            }
            TreeMaker.CreateTree(column, 2, 3, 2);
        }
    }
}