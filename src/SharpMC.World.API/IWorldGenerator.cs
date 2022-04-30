using SharpMC.API.Worlds;

namespace SharpMC.World.API
{
    public interface IWorldGenerator : IWorldProvider
    {
        IChunkColumn GenerateChunkColumn(ICoordinates coordinates);
    }
}