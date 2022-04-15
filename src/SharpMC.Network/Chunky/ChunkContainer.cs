namespace SharpMC.Chunky
{
    public sealed class ChunkContainer
    {
        public DataPalette[] Sections { get; }

        private ChunkContainer(DataPalette[] sections)
        {
            Sections = sections;
        }

        public static ChunkContainer From(DataPalette[] sections)
        {
            return new ChunkContainer(sections);
        }
    }
}