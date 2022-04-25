using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Generators;

namespace SharpMC.World.Nether
{
    internal class NetherWorldProvider : IWorldProvider
    {
        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            throw new System.NotImplementedException();
        }

        public PlayerLocation SpawnPoint { get; }
    }
}