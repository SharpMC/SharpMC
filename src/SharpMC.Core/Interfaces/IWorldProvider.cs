// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Interfaces
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

		public virtual IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			List<Tuple<int, int>> chunksUsed, Player player, bool output = false)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = viewDistance / Math.PI;
				var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				var centerX = (int)(playerX) >> 4;
				var centerZ = (int)(playerZ) >> 4;

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = (x * x) + (z * z);
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

		public virtual void SetBlock(Block block, Level level, bool broadcast)
		{
		}

		public virtual void ClearCache()
		{
		}
	}
}