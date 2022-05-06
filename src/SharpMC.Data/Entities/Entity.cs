using SharpMC.Data.Items;

namespace SharpMC.Data.Entities
{
    public record Entity
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? DisplayName { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
        public EntityType Type { get; init; }
        public LootItem[]? Drops { get; init; }
    }
}