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
         //   var random = new CryptoRandom();
            var blocks = new byte[16 * 16 * 256];
            for (int i = 0; i < (256 * 2); i += 2)
            {
                byte[] Blockie = BitConverter.GetBytes ((ushort)(7 << 4) | 0);
                blocks [i] = Blockie [0];
                blocks [i + 1] = Blockie [1];
            }

            chunk.Blocks = blocks.ToArray();
        }

    }
}

