using System;
using System.Collections.Generic;
using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Interfaces
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

		public virtual IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, Player player, bool output = false)
		{
			throw new NotImplementedException();
		}

		public virtual void SaveChunks(string folder)
		{
			throw new NotImplementedException();
		}

		public virtual ChunkColumn LoadChunk(int x, int y)
		{
			throw new NotImplementedException();
		}

		public virtual void SetBlock(Block block, Level level, bool broadcast)
		{
			//throw new NotImplementedException();
		}
	}
}