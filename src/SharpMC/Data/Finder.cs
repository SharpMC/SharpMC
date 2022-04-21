using System;
using System.Collections.Generic;
using SharpMC.Blocks;

namespace SharpMC.Data
{
    public static class Finder
    {
        private static IDictionary<int, Block> StatesToBlocks { get; } = LoadBlockStates();

        public static Block FindBlockByState(int state)
        {
            if (StatesToBlocks.TryGetValue(state, out var block))
                return block;
            throw new InvalidOperationException($"Could not find block: {state}");
        }

        private static IDictionary<int, Block> LoadBlockStates()
        {
            var dict = new SortedDictionary<int, Block>();
            foreach (var block in KnownBlocks.All)
            {
                for (var stateId = block.MinStateId; stateId <= block.MaxStateId; stateId++)
                {
                    var offset = block.DefaultState;
                    var toAdd = stateId == offset ? block : new MetaBlock(block, stateId - offset);
                    dict.Add(stateId, toAdd);
                }
            }
            return dict;
        }
    }
}