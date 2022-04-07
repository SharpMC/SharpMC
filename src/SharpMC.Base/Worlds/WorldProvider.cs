using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using SharpMC.Networking.Packages;
using SharpMC.World;

namespace SharpMC.Worlds
{
	public class WorldProvider
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

		public IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, List<Tuple<int, int>> chunksUsed, Player player)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = viewDistance / Math.PI;
				var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				var centerX = (int)player.KnownPosition.X >> 4;
				var centerZ = (int)player.KnownPosition.Z >> 4;

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = x * x + z * z;
						if (distance > radiusSquared)
						{
							continue;
						}
						var chunkX = (int)(x + centerX);
						var chunkZ = (int)(z + centerZ);
						var index = new Tuple<int, int>(chunkX, chunkZ);
						newOrders[index] = distance;
					}
				}

				if (newOrders.Count > viewDistance)
				{
					foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
					{
						if (newOrders.Count <= viewDistance) break;
						newOrders.Remove(pair.Key);
					}
				}

				foreach (var chunkKey in chunksUsed.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						chunksUsed.Remove(chunkKey);
						new Task(() => player.UnloadChunk(chunkKey.Item1, chunkKey.Item2)).Start();
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.Contains(pair.Key)) continue;

					var chunk = GenerateChunkColumn(new Vector2(pair.Key.Item1, pair.Key.Item2));
					chunksUsed.Add(pair.Key);

					yield return chunk;
				}
			}
		}

		public virtual IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			List<Tuple<int, int>> chunksUsed, Player player, bool output = false)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = viewDistance / Math.PI;
				var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				var centerX = (int)playerX >> 4;
				var centerZ = (int)playerZ >> 4;

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = x * x + z * z;
						if (distance > radiusSquared)
						{
							continue;
						}
						var chunkX = (int)(x + centerX);
						var chunkZ = (int)(z + centerZ);
						var index = new Tuple<int, int>(chunkX, chunkZ);
						newOrders[index] = distance;
					}
				}

				if (newOrders.Count > viewDistance)
				{
					foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
					{
						if (newOrders.Count <= viewDistance) break;
						newOrders.Remove(pair.Key);
					}
				}

				foreach (var chunkKey in chunksUsed.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						new ChunkData(player.Wrapper)
						{
							Queee = false,
							Unloader = true,
							Chunk = new ChunkColumn { X = chunkKey.Item1, Z = chunkKey.Item2 }
						}.Write();

						chunksUsed.Remove(chunkKey);
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.Contains(pair.Key)) continue;

					var chunk = GenerateChunkColumn(new Vector2(pair.Key.Item1, pair.Key.Item2));
					chunksUsed.Add(pair.Key); 

					yield return chunk;
				}
			}
		}

		public virtual void SaveChunks(string folder)
		{
			throw new NotImplementedException();
		}

		public virtual ChunkColumn LoadChunk(int x, int y)
		{
			throw new NotImplementedException();
		}

		public virtual void ClearCache()
		{
		}
	}
}