namespace SharpMC.World.Standard.API
{
    public interface IStructure
    {
        void Create(IChunkColumn chunk, int x, int y, int z);
    }
}