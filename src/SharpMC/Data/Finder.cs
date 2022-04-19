using System;
using System.Collections.Generic;
using SharpMC.Blocks;

namespace SharpMC.Data
{
    public static class Finder
    {
        private static IDictionary<int, MiBlock> StatesToBlocks { get; } = LoadBlockStates();

        public static MiBlock FindBlockByState(int state)
        {
            if (StatesToBlocks.TryGetValue(state, out var block))
                return block;
            throw new InvalidOperationException($"Could not find block: {state}");
        }

        private static IDictionary<int, MiBlock> LoadBlockStates()
        {
            var dict = new SortedDictionary<int, MiBlock>();
            foreach (var block in KnownBlocks.All)
            {
                for (var stateId = block.MinStateId; stateId <= block.MaxStateId; stateId++)
                {
                    var offset = block.DefaultState;
                    var toAdd = stateId == offset ? block : new MiBlockEx(block, stateId - offset);
                    dict.Add(stateId, toAdd);
                }
            }
            return dict;
        }
    }
}