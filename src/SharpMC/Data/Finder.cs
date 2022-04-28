using System;
using System.Collections.Generic;
using SharpMC.Blocks;
using SharpMC.Items;
using SharpMC.Items.Usables;

namespace SharpMC.Data
{
    public static class Finder
    {
        private static IDictionary<int, Block> StatesToBlocks { get; } = LoadBlockStates();
        private static IDictionary<int, Item> IdsToItems { get; } = LoadItemIds();

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

        public static Item FindItemById(int id)
        {
            if (IdsToItems.TryGetValue(id, out var item))
                return item;
            if (id == 286)
                return new InfoTool();
            throw new InvalidOperationException($"Could not find item: {id}");
        }

        private static IDictionary<int, Item> LoadItemIds()
        {
            var dict = new SortedDictionary<int, Item>();
            foreach (var item in KnownItems.All)
            {
                dict.Add(item.Id, item);
            }
            return dict;
        }
    }
}