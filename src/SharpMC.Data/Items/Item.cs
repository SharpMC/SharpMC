namespace SharpMC.Data.Items
{
    public record Item
    {
        public int Id { get; init; }
        public string? DisplayName { get; init; }
        public string? Name { get; init; }
        public int StackSize { get; init; }
        public ItemType? ItemType { get; init; }
        public ItemMaterial? ItemMaterial { get; init; }
    }
}