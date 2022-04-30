using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.World.API
{
    public interface IWorldProvider
    {
        void PopulateChunk(IChunkColumn chunk, ICoordinates pos);

        ILocation SpawnPoint { get; }
    }
}