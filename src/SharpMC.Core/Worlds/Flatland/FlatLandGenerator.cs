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
using System.IO;
using System.Linq;
using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Interfaces;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Worlds.Flatland
{
	public class FlatLandGenerator : IWorldProvider
	{
		private readonly string Folder = "world";
		public Dictionary<Tuple<int, int>, ChunkColumn> _chunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public FlatLandGenerator(string folder)
		{
			Folder = folder;
			IsCaching = true;

			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
		}

		public override bool IsCaching { get; set; }

		public override void Initialize()
		{
		}

		public override IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, Player player, bool output = false)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = viewDistance/Math.PI;
				var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				var centerX = Math.Floor((playerX)/16);
				var centerZ = Math.Floor((playerZ)/16);

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = (x*x) + (z*z);
						if (distance > radiusSquared)
						{
							continue;
						}
						var chunkX = (int) Math.Floor(x + centerX);
						var chunkZ = (int) Math.Floor(z + centerZ);

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


				foreach (var chunkKey in chunksUsed.Keys.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						new ChunkData(player.Wrapper)
						{
							Queee = false,
							Unloader = true,
							Chunk = new ChunkColumn {X = chunkKey.Item1, Z = chunkKey.Item2}
						}.Write();

						chunksUsed.Remove(chunkKey);
					}
				}

				var stopwatch = new Stopwatch();
				long avarageLoadTime = -1;
				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					stopwatch.Restart();

					var x = pair.Key.Item1;
					var z = pair.Key.Item2;

					var chunk = GenerateChunkColumn(new Vector2(x, z));
					chunksUsed.Add(pair.Key, chunk);

					var elapsed = stopwatch.ElapsedMilliseconds;
					if (avarageLoadTime == -1) avarageLoadTime = elapsed;
					else avarageLoadTime = (avarageLoadTime + elapsed)/2;
					Debug.WriteLine("Chunk {2} generated in: {0} ms (Avarage: {1} ms)", elapsed, avarageLoadTime, pair.Key);

					yield return chunk;
				}
			}
		}

		private string GetChunkHash(double chunkX, double chunkZ)
		{
			return string.Format("{0}:{1}", chunkX, chunkZ);
		}

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			if (_chunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
			{
				ChunkColumn c;
				if (_chunkCache.TryGetValue(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c))
				{
					Debug.WriteLine("Chunk " + chunkCoordinates.X + ":" + chunkCoordinates.Z + " was already generated!");
					return c;
				}
			}

			if (File.Exists((Folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile")))
			{
				var cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				if (!_chunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
					_chunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				return cd;
			}

			Debug.WriteLine("ChunkFile not found, generating...");

			var chunk = new ChunkColumn {X = chunkCoordinates.X, Z = chunkCoordinates.Z};
			var h = PopulateChunk(chunk);

			chunk.SetBlock(0, h + 1, 0, new Block(7));
			chunk.SetBlock(1, h + 1, 0, new Block(41));
			chunk.SetBlock(2, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));
			chunk.SetBlock(3, h + 1, 0, new Block(41));

			if (!_chunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
				_chunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(1, 1, 1);
		}

		public int PopulateChunk(ChunkColumn chunk)
		{
			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					for (var y = 0; y < 4; y++)
					{
						if (y == 0)
						{
							chunk.SetBlock(x, y, z, new BlockBedrock());
						}


						if (y == 1 || y == 2)
						{
							chunk.SetBlock(x, y, z, new Block(3));
						}

						if (y == 3)
						{
							chunk.SetBlock(x, y, z, new BlockGrass());
						}

						if (chunk.X == 1 && chunk.Z == 1)
						{
							if (y == 1 || y == 2 || y == 3)
							{
								chunk.SetBlock(x, y, z, new BlockFlowingLava());
							}
						}

						if (chunk.X == 3 && chunk.Z == 1)
						{
							if (y == 3)
							{
								chunk.SetBlock(x, y, z, new BlockFlowingWater());
							}
						}
					}
				}
			}

			return 4;
		}

		public override void SaveChunks(string folder)
		{
			lock (_chunkCache)
			{
				foreach (var i in _chunkCache.Values.ToArray())
				{
					File.WriteAllBytes(Folder + "/" + i.X + "." + i.Z + ".cfile", Globals.Compress(i.Export()));
				}
			}
		}

		public override ChunkColumn LoadChunk(int x, int z)
		{
			var u = Globals.Decompress(File.ReadAllBytes(Folder + "/" + x + "." + z + ".cfile"));
			var reader = new LocalDataBuffer(u);

			var blockLength = reader.ReadInt();
			var block = reader.ReadUShortLocal(blockLength);

			var metalength = reader.ReadInt();
			var blockmeta = reader.ReadShortLocal(metalength);

			//var blockies = new Block[block.Length];
			//for (var i = 0; i < block.Length; i++)
			//{
			//	blockies[i] = new Block(block[i]) { Metadata = (byte) blockmeta[i] };
			//}

			var skyLength = reader.ReadInt();
			var skylight = reader.Read(skyLength);

			var lightLength = reader.ReadInt();
			var blocklight = reader.Read(lightLength);

			var biomeIdLength = reader.ReadInt();
			var biomeId = reader.Read(biomeIdLength);

			var cc = new ChunkColumn
			{
				Blocks = block,
				Metadata = blockmeta,
				Blocklight = {Data = blocklight},
				Skylight = {Data = skylight},
				BiomeId = biomeId,
				X = x,
				Z = z
			};
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return cc;
		}
	}
}