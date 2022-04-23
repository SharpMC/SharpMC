using SharpMC.Data;
using SharpMC.Items;

namespace SharpMC.Blocks
{
    public class Block : IBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int MinStateId { get; set; }
        public int MaxStateId { get; set; }
        public int DefaultState { get; set; }
        public string Material { get; set; }
        public bool Diggable { get; set; }
        public LootItem[] Drops { get; set; }
        public double Resistance { get; set; }
        public double Hardness { get; set; }
        public int StackSize { get; set; }

        public override string ToString() => Name;

        #region Helpers

        public static Block operator +(Block b, int m)
            => Finder.FindBlockByState(b.DefaultState + m);

        public static Block operator -(Block b, int m)
            => Finder.FindBlockByState(b.DefaultState - m);

        #endregion
    }
}