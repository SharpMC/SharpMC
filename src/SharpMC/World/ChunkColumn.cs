using SharpMC.Blocks;
using SharpMC.Chunky;
using SharpMC.Data;

namespace SharpMC.World
{
    public class ChunkColumn : IChunkColumn
    {
        public int Offset { get; }
        public int Height { get; }
        public int Width { get; }
        public int Depth { get; }
        private byte[] HeightMap { get; }
        private byte[] Biomes { get; }
        private ChunkSection[] Sections { get; set; }
        private byte[] Cache { get; set; }

        public ChunkColumn(int width = 16, int depth = 16, int height = 24, int offset = -16)
        {
            Offset = offset;
            Height = height;
            Width = width;
            Depth = depth;

            HeightMap = new byte[Width * Depth];
            for (var i = 0; i < HeightMap.Length; i++)
                HeightMap[i] = 0;

            Biomes = new byte[Depth * Width];
            for (var i = 0; i < Biomes.Length; i++)
                Biomes[i] = (byte) BiomeIds.Plains;

            Sections = new ChunkSection[height];
            for (var i = 0; i < Sections.Length; i++)
                Sections[i] = Bulk.NewSection(44);
        }

        private ChunkSection GetChunkSection(int y) => Sections[y >> 4];

        private (int x, int, int z) GetIndex(int x, int y, int z)
            => (x, y + Offset * (y >> 4), z);

        private int GetBlockId(int x, int y, int z)
        {
            var section = GetChunkSection(y);
            var index = GetIndex(x, y, z);
            return section[index];
        }

        private void SetBlockId(int x, int y, int z, int id)
        {
            var section = GetChunkSection(y);
            var index = GetIndex(x, y, z);
            section[index] = id;
            Cache = null;
            IsDirty = true;
        }

        private void SetBiome(int x, int z, byte biome)
        {
            Biomes[(z << 4) + x] = biome;
            Cache = null;
        }

        private byte GetBiome(int x, int z) => Biomes[(z << 4) + x];

        private void ReCalcHeight()
        {
            for (var x = 0; x < Width; x++)
            for (var z = 0; z < Depth; z++)
            for (var y = 127; y > 0; y--)
                if (GetBlockId(x, y, z) != 0)
                {
                    HeightMap[(x << 4) + z] = (byte) (y + 1);
                    break;
                }
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            var state = GetBlockId(x, y, z);
            var block = Finder.FindBlockByState(state);
            return block;
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            var state = block.DefaultState;
            SetBlockId(x, y, z, state);
        }

        public byte[] ToArray()
        {
            if (Cache != null)
            {
                return Cache;
            }
            Sections.CompactAirPalette();
            Sections.RecountBlocks();
            var bytes = Chunks.WriteAll(Sections);
            Cache = bytes;
            IsDirty = false;
            return bytes;
        }

        public void FromArray(byte[] bytes)
        {
            var count = Sections.Length;
            Sections = Chunks.ReadAll(bytes, count);
            Cache = bytes;
            IsDirty = false;
        }

        public bool IsDirty { get; private set; }
    }
}