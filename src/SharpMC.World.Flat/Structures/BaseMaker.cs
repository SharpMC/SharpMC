using SharpMC.API.Blocks;
using SharpMC.API.Worlds;

namespace SharpMC.World.Flat.Structures
{
    public static class BaseMaker
    {
        public static void GoSetBlock(this IChunkColumn column, (int x, int y, int z) start,
            (int x, int y, int z) move, IBlock block)
        {
            column.SetBlock(start.x + move.x, start.y + move.y, start.z + move.z, block);
        }
    }
}