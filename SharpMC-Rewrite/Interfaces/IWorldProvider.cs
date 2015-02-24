using System;
using System.Collections.Generic;
using SharpMCRewrite;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Worlds;

namespace MiNET.Worlds
{
	public class IWorldProvider
	{
		public virtual bool IsCaching { get; set; }

		public virtual void Initialize()
		{
		}

		public virtual ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			throw new NotImplementedException();
		}

		public virtual Vector3 GetSpawnPoint()
		{
			throw new NotImplementedException();
		}

		public virtual IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, bool output = false)
		{
			throw new NotImplementedException();
		}

		public virtual void SaveChunks(string Folder)
		{
			throw new NotImplementedException();
		}

		public virtual ChunkColumn LoadChunk(int x, int y)
		{
			throw new NotImplementedException();
		}

		public virtual void SetBlock(Block block, Level level, bool broadcast)
		{
			throw new NotImplementedException();
		}

		public virtual ChunkColumn GetChunk(int x, int z)
		{
			throw new NotImplementedException();
		}
	}
}