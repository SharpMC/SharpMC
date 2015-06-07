#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds.Standard
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using SharpMC.Blocks;
	using SharpMC.Entity;
	using SharpMC.Interfaces;
	using SharpMC.Networking.Packages;
	using SharpMC.Utils;
	using SharpMC.Worlds.Standard.BiomeSystem;
	using SharpMC.Worlds.Standard.Decorators;

	internal class StandardWorldProvider : IWorldProvider
	{
		// World Tweaking settings
		// These settings can be tweaked to changed the look of the terrain.
		private const double OverhangOffset = 32.0; // Old value: 32.0 || Changes the offset from the bottom ground.

		private const double BottomOffset = 42.0; // Old value: 96.0  || Changes the offset from y level 0

		private const double OverhangsMagnitude = 16.0; // Old value: 16.0

		private const double BottomsMagnitude = 32.0; // Old value: 32.0

		private const double OverhangScale = 128.0; // Old value: 128.0 || Changes the scale of the overhang.

		private const double Groundscale = 256.0; // Old value: 256.0   || Changes the scale of the ground.

		private const double Threshold = 0.1; // Old value: 0.0 || Cool value: -0.3 hehehe

		private const double BottomsFrequency = 0.5; // Original 0.5

		private const double BottomsAmplitude = 0.5; // Original 0.5

		private const double OverhangFrequency = 0.5; // Original 0.5

		private const double OverhangAmplitude = 0.5; // Original 0.5

		private const bool EnableOverhang = true; // Enable overhang?

		private static readonly Random Getrandom = new Random();

		private static readonly object SyncLock = new object();

		public static int WaterLevel = 72;

		private readonly BiomeManager _biomeManager;

		private readonly CaveGenerator _cavegen = new CaveGenerator(Globals.Seed.GetHashCode());

		private readonly string _folder;

		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public StandardWorldProvider(string folder)
		{
			this._folder = folder;
			this.IsCaching = true;
			this._biomeManager = new BiomeManager(Globals.Seed.GetHashCode());

			// 	_biomeManager.AddBiomeType(new OceanBiome()); //Not adding until we fixed the transitions :(
			this._biomeManager.AddBiomeType(new FlowerForestBiome());
			this._biomeManager.AddBiomeType(new ForestBiome());
			this._biomeManager.AddBiomeType(new BirchForestBiome());
			this._biomeManager.AddBiomeType(new PlainsBiome());
			this._biomeManager.AddBiomeType(new DesertBiome());
			this._biomeManager.AddBiomeType(new SunFlowerPlainsBiome());
		}

		public override sealed bool IsCaching { get; set; }

		public override ChunkColumn LoadChunk(int x, int z)
		{
			var u = Globals.Decompress(File.ReadAllBytes(this._folder + "/" + x + "." + z + ".cfile"));
			var reader = new LocalDataBuffer(u);

			var blockLength = reader.ReadInt();
			var block = reader.ReadUShortLocal(blockLength);

			var metalength = reader.ReadInt();
			var blockmeta = reader.ReadShortLocal(metalength);

			// var blockies = new Block[block.Length];
			// var blocks = new ushort[block.Length];
			// for (var i = 0; i < block.Length; i++)
			// {
			// 	blockies[i] = new Block(block[i]) {Metadata = (byte) blockmeta[i]};
			// }
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
					         Blocklight = {
                              Data = blocklight 
                           }, 
					         Skylight = {
                            Data = skylight 
                         }, 
					         BiomeId = biomeId, 
					         X = x, 
					         Z = z
				         };
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return cc;

			// throw new NotImplementedException();
		}

		public override void SaveChunks(string folder)
		{
			lock (this.ChunkCache)
			{
				foreach (var i in this.ChunkCache.Values.ToArray())
				{
					File.WriteAllBytes(this._folder + "/" + i.X + "." + i.Z + ".cfile", Globals.Compress(i.Export()));
				}
			}
		}

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			ChunkColumn c;
			if (this.ChunkCache.TryGetValue(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c))
			{
				return c;
			}

			if (File.Exists(this._folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile"))
			{
				var cd = this.LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				lock (this.ChunkCache)
				{
					if (!this.ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
					{
						this.ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
					}
				}

				return cd;
			}

			var chunk = new ChunkColumn { X = chunkCoordinates.X, Z = chunkCoordinates.Z };
			this.PopulateChunk(chunk);

			if (!this.ChunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
			{
				this.ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);
			}

			return chunk;
		}

		public override IEnumerable<ChunkColumn> GenerateChunks(
			int viewDistance, 
			double playerX, 
			double playerZ, 
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, 
			Player player, 
			bool output = false)
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
						if (newOrders.Count <= viewDistance)
						{
							break;
						}

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
								Chunk = new ChunkColumn { X = chunkKey.Item1, Z = chunkKey.Item2 }
							}.Write();

						chunksUsed.Remove(chunkKey);
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key))
					{
						continue;
					}

					var chunk = this.GenerateChunkColumn(new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2));
					chunksUsed.Add(pair.Key, chunk);

					yield return chunk;
				}

				if (chunksUsed.Count > viewDistance)
				{
					Debug.WriteLine("Too many chunks used: {0}", chunksUsed.Count);
				}
			}
		}

		public static int GetRandomNumber(int min, int max)
		{
			lock (SyncLock)
			{
				return Getrandom.Next(min, max);
			}
		}

		private void PopulateChunk(ChunkColumn chunk)
		{
			var bottom = new SimplexOctaveGenerator(Globals.Seed.GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator(Globals.Seed.GetHashCode(), 8);
			overhang.SetScale(1 / OverhangScale);
			bottom.SetScale(1 / Groundscale);

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X * 16;
					float oz = z + chunk.Z * 16;

					var cBiome = this._biomeManager.GetBiome((int)ox, (int)oz);
					chunk.BiomeId[x * 16 + z] = cBiome.MinecraftBiomeId;

					var bottomHeight =
						(int)
						((bottom.Noise(ox, oz, BottomsFrequency, BottomsAmplitude) * BottomsMagnitude) + BottomOffset + cBiome.BaseHeight);

					var maxHeight =
						(int)
						((overhang.Noise(ox, oz, OverhangFrequency, OverhangAmplitude) * OverhangsMagnitude) + bottomHeight
						 + OverhangOffset);
					maxHeight = Math.Max(1, maxHeight);

					for (var y = 0; y < maxHeight && y < 256; y++)
					{
						if (y == 0)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(7));
							continue;
						}

						if (y > bottomHeight)
						{
							// part where we do the overhangs
							if (EnableOverhang)
							{
								var density = overhang.Noise(ox, y, oz, OverhangFrequency, OverhangAmplitude);
								if (density > Threshold)
								{
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
								}
							}
						}
						else
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
						}
					}

					// Turn the blocks ontop into the correct material
					for (var y = 0; y < 256; y++)
					{
						if (chunk.GetBlock(x, y + 1, z) == 0 && chunk.GetBlock(x, y, z) == 1)
						{
							chunk.SetBlock(x, y, z, cBiome.TopBlock);

							chunk.SetBlock(x, y - 1, z, cBiome.Filling);
							chunk.SetBlock(x, y - 2, z, cBiome.Filling);
						}
					}

					foreach (var decorator in cBiome.Decorators)
					{
						decorator.Decorate(chunk, cBiome, x, z);
					}

					new OreDecorator().Decorate(chunk, cBiome, x, z); // Ores :)
					new BedrockDecorator().Decorate(chunk, cBiome, x, z); // Random bedrock :)
				}
			}

			new WaterDecorator().Decorate(chunk, new PlainsBiome()); // For now, ALWAYS use the water decorator on all chunks...
			this._cavegen.GenerateCave(chunk);
			new LavaDecorator().Decorate(chunk, new PlainsBiome());
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			lock (this.ChunkCache)
			{
				if (
					!this.ChunkCache.TryGetValue(
						new Tuple<int, int>((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4), 
						out c))
				{
					throw new Exception("No chunk found!");
				}
			}

			c.SetBlock(
				(int)block.Coordinates.X & 0x0f, 
				(int)block.Coordinates.Y & 0x7f, 
				(int)block.Coordinates.Z & 0x0f, 
				block);
			if (!broadcast)
			{
				return;
			}

			BlockChange.Broadcast(block, level);
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(0, 82, 0);
		}

		public override void ClearCache()
		{
			lock (this.ChunkCache)
			{
				this.ChunkCache.Clear();
			}
		}
	}
}