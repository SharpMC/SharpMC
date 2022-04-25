using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Generators;

namespace SharpMC.World.Standard
{
    internal class StandardWorldProvider : IWorldProvider
    {
        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            throw new System.NotImplementedException();
        }

        public PlayerLocation SpawnPoint { get; }
    }
}