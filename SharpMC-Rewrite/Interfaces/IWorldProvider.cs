using SharpMCRewrite.Worlds;
using SharpMCRewrite;
using System.Collections.Generic;
using System;

namespace MiNET.Worlds
{
	public interface IWorldProvider
	{
		bool IsCaching { get; }
		void Initialize();
        ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates);
        Vector3 GetSpawnPoint();
        IEnumerable<ChunkColumn> GenerateChunks (int _viewDistance, double playerX, double playerZ, Dictionary<Tuple<int,int>, ChunkColumn> chunksUsed);
        void SaveChunks(string Folder);
        ChunkColumn LoadChunk(int x, int y);
        void SetBlock (Vector3 cords, ushort blockID);
    }
}