using SharpMC.Players;
using SharpMC.Util;

namespace SharpMC.World.Generators
{
    public interface IWorldGenerator
    {
        IChunkColumn GenerateChunkColumn(ChunkCoordinates coordinates);

        PlayerLocation GetSpawnPoint();
    }
}