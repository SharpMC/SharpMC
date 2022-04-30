using SharpMC.API.Blocks;
using SharpMC.Data.Items;

namespace SharpMC.Data.Blocks
{
    public record Block : IBlock
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? DisplayName { get; init; }
        public int MinStateId { get; init; }
        public int MaxStateId { get; init; }
        public bool Diggable { get; init; }
        public double Hardness { get; init; }
        public double Resistance { get; init; }
        public int StackSize { get; init; }
        public int DefaultState { get; init; }
        public string? Material { get; init; }
        public LootItem[]? Drops { get; init; }
        public bool? IsReplaceable { get; init; }
        public bool? IsSolid { get; init; }
        public int? Meta { get; init; }

        #region Helpers
        public static Block operator +(Block b, int m)
            => Finder.FindBlockByState(b.DefaultState + m);
        public static Block operator -(Block b, int m)
            => Finder.FindBlockByState(b.DefaultState - m);
        public virtual bool Equals(IBlock? other)
            => Id == other?.Id && DefaultState == other.DefaultState;
        #endregion
    }
}