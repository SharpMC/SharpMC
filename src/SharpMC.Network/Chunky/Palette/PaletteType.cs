namespace SharpMC.Network.Chunky.Palette
{
    public readonly struct PaletteType
    {
        public static readonly PaletteType Biome = new(1, 3, 64);

        public static readonly PaletteType Chunk = new(4, 8, 4096);

        public int MinBitsPerEntry { get; }
        public int MaxBitsPerEntry { get; }
        public int StorageSize { get; }

        private PaletteType(int minBitsPerEntry, int maxBitsPerEntry, int storageSize)
        {
            MinBitsPerEntry = minBitsPerEntry;
            MaxBitsPerEntry = maxBitsPerEntry;
            StorageSize = storageSize;
        }
    }
}