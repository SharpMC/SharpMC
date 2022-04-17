using SharpMC.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class ChunkSection
    {
        public const int Air = 0;

        public int BlockCount { get; set; }
        public DataPalette ChunkData { get; }
        public DataPalette BiomeData { get; }

        public ChunkSection()
            : this(0, DataPalette.CreateForChunk(), DataPalette.CreateForBiome())
        {
        }

        public ChunkSection(int blockCount, DataPalette chunkData, DataPalette biomeData)
        {
            BlockCount = blockCount;
            ChunkData = chunkData;
            BiomeData = biomeData;
        }

        public static ChunkSection Read(IMinecraftReader input)
        {
            int blockCount = input.ReadShort();
            const int chunkBits = DataPalette.GlobalPaletteBitsPerEntry;
            const int biomeBits = 4;
            var chunkPalette = DataPalette.Read(input, PaletteType.Chunk, chunkBits);
            var biomePalette = DataPalette.Read(input, PaletteType.Biome, biomeBits);
            return new ChunkSection(blockCount, chunkPalette, biomePalette);
        }

        public static void Write(IMinecraftWriter output, ChunkSection section)
        {
            output.WriteShort((short) section.BlockCount);
            DataPalette.Write(output, section.ChunkData);
            DataPalette.Write(output, section.BiomeData);
        }

        public int GetBlock(int x, int y, int z)
        {
            return ChunkData.Get(x, y, z);
        }

        public void SetBlock(int x, int y, int z, int state)
        {
            var curr = ChunkData.Set(x, y, z, state);
            if (state != Air && curr == Air)
            {
                BlockCount++;
            }
            else if (state == Air && curr != Air)
            {
                BlockCount--;
            }
        }

        public bool IsBlockCountEmpty => BlockCount == 0;
    }
}