using System.Collections.Generic;
using System.Linq;
using SharpMC.Blocks;

namespace SharpMC.Data
{
    public static class Finder
    {
        private static Dictionary<int, MiBlock> StatesToBlocks { get; } =
            KnownBlocks.All.ToDictionary(k => k.DefaultState, v => v);

        public static MiBlock FindBlockByState(int state)
        {
            StatesToBlocks.TryGetValue(state, out var block);
            return block;
        }
    }
}