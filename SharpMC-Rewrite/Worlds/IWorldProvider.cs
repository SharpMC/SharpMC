using SharpMCRewrite.Worlds;
using SharpMCRewrite;

namespace MiNET.Worlds
{
	public interface IWorldProvider
	{
		bool IsCaching { get; }
		void Initialize();
        ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates);
        Vector3 GetSpawnPoint();
	}
}