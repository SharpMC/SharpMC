namespace SharpMC.Data.Particles
{
    public record Particle
    {
        public int Id { get; init; }
        public string? Name { get; init; }
    }
}