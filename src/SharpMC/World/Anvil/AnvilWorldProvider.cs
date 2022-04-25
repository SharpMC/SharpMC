using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World.Generators;

namespace SharpMC.World.Anvil
{
    internal class AnvilWorldProvider : IWorldProvider
    {
        public void PopulateChunk(IChunkColumn chunk, ChunkCoordinates pos)
        {
            throw new System.NotImplementedException();
        }

        public PlayerLocation SpawnPoint
            => throw new System.NotImplementedException();
    }
}