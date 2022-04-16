namespace SharpMC.Blocks
{
    public class MiBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int MinStateId { get; set; }
        public int MaxStateId { get; set; }
        public int DefaultState { get; set; }
        public string Material { get; set; }
    }
}