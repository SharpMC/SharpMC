namespace SharpMC.Entities
{
    public class MiEntity
    {
        public int Id { get; set; }
        public EntityType Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}