using SharpMC.Players;

namespace SharpMC.World.Generators
{
    public interface IWorldProvider
    {
        void PopulateChunk(IChunkColumn chunk);

        PlayerLocation SpawnPoint { get; }
    }
}