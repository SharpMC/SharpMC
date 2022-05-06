using System.Collections.Generic;

namespace SharpMC.Generator.Prismarine.Data
{
    internal class BlockLoot
    {
        public string Block { get; set; }

        public List<LootItem> Drops { get; set; }
    }
}