namespace SharpMC.Generator.Prismarine.Data
{
    internal class Entity
    {
        public int Id { get; set; }
        public int InternalId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public EntityType Type { get; set; }
        public EntityCategory Category { get; set; }
    }
}