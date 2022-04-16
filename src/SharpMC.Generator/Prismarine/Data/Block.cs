using System.Collections.Generic;

namespace SharpMC.Generator.Prismarine.Data
{
    internal class Block
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public double Hardness { get; set; }
        public double Resistance { get; set; }
        public int MinStateId { get; set; }
        public int MaxStateId { get; set; }
        public int[] Drops { get; set; }
        public bool Diggable { get; set; }
        public bool Transparent { get; set; }
        public bool FilterLight { get; set; }
        public bool EmitLight { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public int StackSize { get; set; }
        public string Material { get; set; }
        public int DefaultState { get; set; }
        public Dictionary<int, bool> HarvestTools { get; set; }
        public BlockState[] States { get; set; }
    }
}