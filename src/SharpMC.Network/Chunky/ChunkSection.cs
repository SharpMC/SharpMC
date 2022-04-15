using SharpMC.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class ChunkSection
    {
        public const int Air = 0;

        public int BlockCount { get; private set; }
        public DataPalette ChunkData { get; }
        public DataPalette BiomeData { get; }

        public ChunkSection()
            : this(0, DataPalette.CreateForChunk(), DataPalette.CreateForBiome(4))
        {
        }

        public ChunkSection(int blockCount, DataPalette chunkData, DataPalette biomeData)
        {
            BlockCount = blockCount;
            ChunkData = chunkData;
            BiomeData = biomeData;
        }

        public static ChunkSection Read(IMinecraftReader input, int globalBiomePaletteBits)
        {
            int blockCount = input.ReadShort();
            var chunkPalette = DataPalette.Read(input, PaletteType.Chunk,
                DataPalette.GlobalPaletteBitsPerEntry);
            var biomePalette = DataPalette.Read(input, PaletteType.Biome,
                globalBiomePaletteBits);
            return new ChunkSection(blockCount, chunkPalette, biomePalette);
        }

        public static void Write(IMinecraftWriter output, ChunkSection section,
            int globalBiomePaletteBits)
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