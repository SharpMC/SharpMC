using SharpMC.API.Worlds;

namespace SharpMC.World.API.Structures
{
    public interface IStructure
    {
        void Create(IChunkColumn chunk, int x, int y, int z);
    }
}