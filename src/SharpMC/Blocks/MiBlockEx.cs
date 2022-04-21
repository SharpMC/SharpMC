namespace SharpMC.Blocks
{
    public class MetaBlock : Block
    {
        public Block Parent { get; }
        public int Meta { get; }

        public MetaBlock(Block block, int meta)
        {
            Parent = block;
            Meta = meta;
            DefaultState = block.DefaultState + meta;
            Id = block.Id;
            Name = block.Name;
            DisplayName = block.DisplayName;
            MinStateId = block.MinStateId;
            MaxStateId = block.MaxStateId;
            Material = block.Material;
        }

        public override string ToString() => $"{Name}#{Meta}";
    }
}