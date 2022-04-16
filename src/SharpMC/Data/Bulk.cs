using SharpMC.Blocks;
using SharpMC.Chunky;

namespace SharpMC.Data
{
    public static class Bulk
    {
        public static void SetBlocks(this ChunkSection section, int x, int z, params (int y, MiBlock block)[] pairs)
        {
            foreach (var (y, block) in pairs)
            {
                var state = block.DefaultState;
                section.SetBlock(x, y, z, state);
            }
        }
    }
}