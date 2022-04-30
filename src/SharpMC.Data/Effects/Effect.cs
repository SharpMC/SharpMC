namespace SharpMC.Data.Effects
{
    public record Effect
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? DisplayName { get; init; }
        public EffectType Type { get; init; }
    }
}