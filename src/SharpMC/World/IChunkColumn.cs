using SharpMC.Blocks;

namespace SharpMC.World
{
    public interface IChunkColumn
    {
        int Height { get; }
        int Width { get; }
        int Depth { get; }

        bool IsDirty { get; }

        IBlock GetBlock(int x, int y, int z);
        void SetBlock(int x, int y, int z, IBlock block);

        byte[] ToArray();

        void SetBiomeId(int index, BiomeIds biomeId);
    }
}