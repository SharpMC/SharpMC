using SharpMC.API.Worlds;
using static SharpMC.Data.Blocks.KnownBlocks;

namespace SharpMC.World.Flat.Structures
{
    public static class TreeMaker
    {
        public static void CreateTree(IChunkColumn column, int sx, int sy, int sz)
        {
            var index = (sx, sy, sz);
            column.GoSetBlock(index, (0, 0, 0), BirchLog);
            column.GoSetBlock(index, (0, 1, 0), BirchLog);
            column.GoSetBlock(index, (0, 2, 0), BirchLog);
            column.GoSetBlock(index, (0, 3, 0), BirchLog);
            column.GoSetBlock(index, (0, 4, 0), BirchLeaves);
            column.GoSetBlock(index, (0, 4, 1), BirchLeaves);
            column.GoSetBlock(index, (1, 4, 0), BirchLeaves);
            column.GoSetBlock(index, (1, 4, 1), BirchLeaves);
            column.GoSetBlock(index, (0, 4, -1), BirchLeaves);
            column.GoSetBlock(index, (-1, 4, 0), BirchLeaves);
            column.GoSetBlock(index, (-1, 4, -1), BirchLeaves);
            column.GoSetBlock(index, (0, 5, 1), BirchLeaves);
            column.GoSetBlock(index, (1, 5, 0), BirchLeaves);
            column.GoSetBlock(index, (1, 5, 1), BirchLeaves);
            column.GoSetBlock(index, (0, 5, -1), BirchLeaves);
            column.GoSetBlock(index, (-1, 5, 0), BirchLeaves);
            column.GoSetBlock(index, (-1, 5, -1), BirchLeaves);
        }
    }
}