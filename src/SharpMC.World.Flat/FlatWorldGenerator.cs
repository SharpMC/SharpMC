using System.Collections.Concurrent;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;
using SharpMC.World.API;
using SharpMC.World.API.Chunks;
using SharpMC.World.Flat.Structures;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Flat
{
    public class FlatWorldGenerator : IWorldGenerator
    {
        private ConcurrentDictionary<ICoordinates, IChunkColumn> Chunks { get; }

        private readonly IChunkColumnFactory _factory;

        public FlatWorldGenerator(IChunkColumnFactory factory)
        {
            _factory = factory;
            Chunks = new ConcurrentDictionary<ICoordinates, IChunkColumn>();
        }

        public IChunkColumn GenerateChunkColumn(ICoordinates coordinates)
        {
            return Chunks.GetOrAdd(coordinates, CreateChunk);
        }

        public ILocation SpawnPoint
            => new PlayerLocation(0.5, 4f, 0.5f);

        public void PopulateChunk(IChunkColumn chunk, ICoordinates pos)
            => CreateChunk(chunk, pos);

        private IChunkColumn CreateChunk(ICoordinates _)
        {
            IChunkColumn column = _factory.CreateColumn();
            CreateChunk(column, _);
            return column;
        }

        private static void CreateChunk(IChunkColumn column, ICoordinates _)
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