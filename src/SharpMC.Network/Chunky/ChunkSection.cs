using System;
using SharpMC.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class ChunkSection : IEquatable<ChunkSection>
    {
        private const int Air = 0;

        public int BlockCount { get; set; }
        public DataPalette ChunkData { get; }
        public DataPalette BiomeData { get; }

        public ChunkSection(int blockCount, DataPalette chunkData, DataPalette biomeData)
        {
            BlockCount = blockCount;
            ChunkData = chunkData ?? throw new ArgumentException("chunkData is marked non-null but is null");
            BiomeData = biomeData ?? throw new ArgumentException("biomeData is marked non-null but is null");
        }

        public ChunkSection()
            : this(0, DataPalette.CreateForChunk(), DataPalette.CreateForBiome())
        {
        }

        public static ChunkSection Read(IMinecraftReader input)
        {
            int blockCount = input.ReadShort();
            var chunkPalette = DataPalette.Read(input, PaletteType.Chunk,
                DataPalette.GlobalPaletteBitsPerEntry);
            var biomePalette = DataPalette.Read(input, PaletteType.Biome,
                DataPalette.GlobalBiomeBitsPerEntry);
            return new ChunkSection(blockCount, chunkPalette, biomePalette);
        }

        public static void Write(IMinecraftWriter output, ChunkSection section)
        {
            output.WriteShort((short) section.BlockCount);
            DataPalette.Write(output, section.ChunkData);
            DataPalette.Write(output, section.BiomeData);
        }

        public int this[(int x, int y, int z) index]
        {
            get => ChunkData[index];
            set
            {
                var curr = ChunkData[index];
                ChunkData[index] = value;
                if (value != Air && curr == Air)
                {
                    BlockCount++;
                }
                else if (value == Air && curr != Air)
                {
                    BlockCount--;
                }
            }
        }

        public bool IsBlockCountEmpty => BlockCount == 0;

        public override string ToString()
        {
            return $"ChunkSection(blockCount={BlockCount}, chunkData={ChunkData}," +
                   $" biomeData={BiomeData})";
        }

        #region Hashcode

        public bool Equals(ChunkSection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return BlockCount == other.BlockCount &&
                   Equals(ChunkData, other.ChunkData) &&
                   Equals(BiomeData, other.BiomeData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ChunkSection) obj);
        }

        public override int GetHashCode()
            => HashCode.Combine(BlockCount, ChunkData, BiomeData);

        public static bool operator ==(ChunkSection left, ChunkSection right)
            => Equals(left, right);

        public static bool operator !=(ChunkSection left, ChunkSection right)
            => !Equals(left, right);

        #endregion
    }
}