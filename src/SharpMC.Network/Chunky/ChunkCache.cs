using System;
using System.Collections.Generic;
using SharpMC.Network.Chunky.Utils;

namespace SharpMC.Chunky
{
    public sealed class ChunkCache : IDisposable
    {
        private readonly IDictionary<long, ChunkContainer> _chunks;

        private int _minY;
        private int _heightY;

        public ChunkCache()
        {
            _chunks = new Dictionary<long, ChunkContainer>();
        }

        public void AddToCache(int x, int z, DataPalette[] chunks)
        {
            var chunkPosition = MathUtils.ChunkPositionToLong(x, z);
            var chunk = ChunkContainer.From(chunks);
            _chunks.Add(chunkPosition, chunk);
        }

        private ChunkContainer GetChunk(int x, int z)
        {
            var chunkPosition = MathUtils.ChunkPositionToLong(x, z);
            _chunks.TryGetValue(chunkPosition, out var value);
            return value;
        }

        public void RemoveChunk(int x, int z)
        {
            var chunkPosition = MathUtils.ChunkPositionToLong(x, z);
            _chunks.Remove(chunkPosition);
        }

        public int GetBlockAt(int x, int y, int z)
        {
            var column = GetChunk(x >> 4, z >> 4);
            if (column == null)
            {
                return BlockStateValues.JavaAirId;
            }
            if (y < _minY || ((y - _minY) >> 4) > column.Sections.Length - 1)
            {
                return BlockStateValues.JavaAirId;
            }
            var chunk = column.Sections[(y - _minY) >> 4];
            if (chunk != null)
            {
                return chunk.Get(x & 0xF, y & 0xF, z & 0xF);
            }
            return BlockStateValues.JavaAirId;
        }

        public void UpdateBlock(int x, int y, int z, int block)
        {
            var chunk = GetChunk(x >> 4, z >> 4);
            if (chunk == null)
            {
                return;
            }
            if (y < _minY || ((y - _minY) >> 4) > chunk.Sections.Length - 1)
            {
                return;
            }
            var palette = chunk.Sections[(y - _minY) >> 4];
            if (palette == null)
            {
                if (block != BlockStateValues.JavaAirId)
                {
                    palette = DataPalette.CreateForChunk();
                    palette.Palette.StateToId(BlockStateValues.JavaAirId);
                    chunk.Sections[(y - _minY) >> 4] = palette;
                }
                else
                {
                    return;
                }
            }
            palette.Set(x & 0xF, y & 0xF, z & 0xF, block);
        }

        public void Clear()
        {
            _chunks.Clear();
        }

        public int ChunkMinY
        {
            get => _minY >> 4;
            set => _minY = value;
        }

        public int ChunkHeightY
        {
            get => _heightY >> 4;
            set => _heightY = value;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}