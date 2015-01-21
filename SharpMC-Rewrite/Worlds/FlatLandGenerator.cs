using System;
using System.Collections.Generic;
using System.Linq;
using MiNET.Utils;

namespace SharpMCRewrite.Worlds
{
    public class FlatLandGenerator
    {
        private List<ChunkColumn> _chunkCache = new List<ChunkColumn>();
        public bool IsCaching { get; private set; }

        public FlatLandGenerator()
        {
            IsCaching = true;
        }

        public void Initialize()
        {
        }

        public ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
        {
            var firstOrDefault = _chunkCache.FirstOrDefault(chunk2 => chunk2 != null && chunk2.X == chunkCoordinates.X && chunk2.Y == chunkCoordinates.Y);
            if (firstOrDefault != null)
            {
                return firstOrDefault;
            }

            var generator = new FlatLandGenerator();

            var chunk = new ChunkColumn();
            chunk.X = chunkCoordinates.X;
            chunk.Y = chunkCoordinates.Y;
            generator.PopulateChunk(chunk);

           // chunk.SetBlock(0, 5, 0, 7);
           // chunk.SetBlock(1, 5, 0, 41);
           // chunk.SetBlock(2, 5, 0, 41);
           // chunk.SetBlock(3, 5, 0, 41);
           // chunk.SetBlock(3, 5, 0, 41);

            _chunkCache.Add(chunk);

            return chunk;
        }

        public Vector3 GetSpawnPoint()
        {
            return new Vector3(1, 1, 1);
        }

        public void PopulateChunk(ChunkColumn chunk)
        {
            var random = new CryptoRandom();
            var blocks = new byte[16 * 16 * 256];
            var meta = new byte[16 * 16 * 256];

            for (int i = 0; i < 256; i++)
            {
                blocks[i] = 7; // Bedrock
                meta [i] = 0;
            }
            chunk.Blocks = blocks;
            chunk.Metadata = meta;



            //chunk.biomeColor = ArrayOf<int>.Create(256, random.Next(6761930, 8761930));
            //          for (int i = 0; i < chunk.biomeColor.Length; i++)
            //          {
            //              chunk.biomeColor[i] = random.Next(6761930, 8761930);
            //          }
        }

    }
}

