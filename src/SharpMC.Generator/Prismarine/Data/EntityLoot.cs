using System.Collections.Generic;

namespace SharpMC.Generator.Prismarine.Data
{
    internal class EntityLoot
    {
        public string Entity { get; set; }

        public List<LootItem> Drops { get; set; }
    }
}