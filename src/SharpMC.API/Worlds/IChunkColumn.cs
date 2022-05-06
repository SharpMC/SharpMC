using SharpMC.API.Blocks;

namespace SharpMC.API.Worlds
{
    public interface IChunkColumn
    {
        IBlock GetBlock(int x, int y, int z);

        void SetBlock(int x, int y, int z, IBlock block);

        void SetBiomeId(int index, BiomeIds biome);

        bool IsDirty { get; }

        int Height { get; }
        int Width { get; }
        int Depth { get; }

        byte[] ToArray();
        void FromArray(byte[] input);

        void AddAirToPalette();
    }
}