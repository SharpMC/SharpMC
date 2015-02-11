using SharpMCRewrite.Worlds;
using SharpMCRewrite;
using System.Collections.Generic;
using System;
using SharpMCRewrite.Blocks;

namespace MiNET.Worlds
{
	public interface IWorldProvider
	{
		bool IsCaching { get; }
		void Initialize();
        ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates);
        Vector3 GetSpawnPoint();
        IEnumerable<ChunkColumn> GenerateChunks (int _viewDistance, double playerX, double playerZ, Dictionary<Tuple<int,int>, ChunkColumn> chunksUsed, ClientWrapper wrapper);
        void SaveChunks(string Folder);
        ChunkColumn LoadChunk(int x, int y);
        void SetBlock (INTVector3 cords, Block block, Level level, bool broadcast);
		ChunkColumn GetChunk(int x, int z);
	}
}