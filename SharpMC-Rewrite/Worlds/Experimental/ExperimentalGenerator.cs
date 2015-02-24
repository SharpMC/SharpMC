using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MiNET.Worlds;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Networking.Packages;
using SimplexNoise;

namespace SharpMCRewrite.Worlds.Experimental
{
	internal class ExperimentalGenerator : IWorldProvider
	{
		private static readonly int _seedoffset = new Random(Globals.Seed.GetHashCode()).Next(1, Int16.MaxValue);
		private static readonly Random getrandom = new Random();
		private static readonly object syncLock = new object();
		private static readonly OpenSimplexNoise OpenNoise = new OpenSimplexNoise(Globals.Seed.GetHashCode());
		private static readonly PerlinNoise PerlinNoise = new PerlinNoise(Globals.Seed.GetHashCode());
		private readonly string _folder = "";
		private readonly float dirtBaseHeight = 1;
		private readonly float dirtNoise = 0.004f;
		private readonly float dirtNoiseHeight = 3;
		private readonly float stoneBaseHeight = 0;
		private readonly float stoneBaseNoise = 0.05f;
		private readonly float stoneBaseNoiseHeight = 4;
		private readonly float stoneMinHeight = 0;
		private readonly float stoneMountainFrequency = 0.008f;
		private readonly float stoneMountainHeight = 48;
		private readonly int waterLevel = 25;
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();
		private List<Vector2> TreesDone = new List<Vector2>();

		public ExperimentalGenerator(string folder)
		{
			_folder = folder;
			IsCaching = true;
		}

		public override bool IsCaching { get; set; }

		public override ChunkColumn GetChunk(int x, int z)
		{
			foreach (var ch in ChunkCache)
			{
				if (ch.Key.Item1 == x && ch.Key.Item2 == z)
				{
					return ch.Value;
				}
			}
			throw new Exception("We couldn't find the chunk.");
		}

		public override ChunkColumn LoadChunk(int x, int z)
		{
			var u = Globals.Decompress(File.ReadAllBytes(_folder + "/" + x + "." + z + ".cfile"));
			var reader = new MSGBuffer(u);

			var BlockLength = reader.ReadInt();
			var Block = reader.ReadUShortLocal(BlockLength);

			var SkyLength = reader.ReadInt();
			var Skylight = reader.Read(SkyLength);

			var LightLength = reader.ReadInt();
			var Blocklight = reader.Read(LightLength);

			var BiomeIDLength = reader.ReadInt();
			var BiomeID = reader.Read(BiomeIDLength);

			var CC = new ChunkColumn();
			CC.Blocks = Block;
			CC.Blocklight.Data = Blocklight;
			CC.Skylight.Data = Skylight;
			CC.BiomeId = BiomeID;
			CC.X = x;
			CC.Z = z;
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return CC;
		}

		public override void SaveChunks(string Folder)
		{
			foreach (var i in ChunkCache)
			{
				File.WriteAllBytes(_folder + "/" + i.Value.X + "." + i.Value.Z + ".cfile", Globals.Compress(i.Value.Export()));
			}
		}

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			if (ChunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
			{
				ChunkColumn c;
				if (ChunkCache.TryGetValue(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c))
				{
					Debug.WriteLine("Chunk " + chunkCoordinates.X + ":" + chunkCoordinates.Z + " was already generated!");
					return c;
				}
			}

			if (File.Exists((_folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile")))
			{
				var cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
					ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				return cd;
			}

			Debug.WriteLine("ChunkFile not found, generating...");

			var chunk = new ChunkColumn {X = chunkCoordinates.X, Z = chunkCoordinates.Z};
			PopulateChunk(chunk);

			ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public override IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, bool output = false)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = _viewDistance/Math.PI;
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

				if (newOrders.Count > _viewDistance)
				{
					foreach (var pair in newOrders.OrderByDescending(pair => pair.Value))
					{
						if (newOrders.Count <= _viewDistance) break;
						newOrders.Remove(pair.Key);
					}
				}


				foreach (var chunkKey in chunksUsed.Keys.ToArray())
				{
					if (!newOrders.ContainsKey(chunkKey))
					{
						chunksUsed.Remove(chunkKey);
					}
				}

				long avarageLoadTime = -1;
				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					var x = pair.Key.Item1;
					var z = pair.Key.Item2;

					var chunk = GenerateChunkColumn(new Vector2(x, z));
					chunksUsed.Add(pair.Key, chunk);

					yield return chunk;
				}
			}
		}

		public static int GetRandomNumber(int min, int max)
		{
			lock (syncLock)
			{
				// synchronize
				return getrandom.Next(min, max);
			}
		}

		private void PopulateChunk(ChunkColumn chunk)
		{
			var trees = new Random().Next(0, 10);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = new Random().Next(1, 16);
				var z = new Random().Next(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					var stoneHeight = (int) Math.Floor(stoneBaseHeight);
					stoneHeight += GetNoise(chunk.X*16 + x, chunk.Z*16 + z, stoneMountainFrequency,
						(int) Math.Floor(stoneMountainHeight));

					if (stoneHeight < stoneMinHeight)
						stoneHeight = (int) Math.Floor(stoneMinHeight);

					stoneHeight += GetNoise(chunk.X*16 + x, chunk.Z*16 + z, stoneBaseNoise, (int) Math.Floor(stoneBaseNoiseHeight));

					var dirtHeight = stoneHeight + (int) Math.Floor(dirtBaseHeight);
					dirtHeight += GetNoise(chunk.X*16 + x, chunk.Z*16 + z, dirtNoise, (int) Math.Floor(dirtNoiseHeight));

					for (var y = 0; y < 256; y++)
					{
						//float y2 = Get3DNoise(chunk.X*16, y, chunk.Z*16, stoneBaseNoise, (int) Math.Floor(stoneBaseNoiseHeight));
						if (y <= stoneHeight)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));

							//Diamond ore
							if (GetRandomNumber(0, 2500) < 5)
							{
								chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(56));
							}

							//Coal Ore
							if (GetRandomNumber(0, 1500) < 50)
							{
								chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(16));
							}

							//Iron Ore
							if (GetRandomNumber(0, 2500) < 30)
							{
								chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(15));
							}

							//Gold Ore
							if (GetRandomNumber(0, 2500) < 20)
							{
								chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(14));
							}
						}

						if (y < waterLevel) //Water :)
						{
							if (chunk.GetBlock(x, y, z) >> 4 == 2 || chunk.GetBlock(x, y, z) >> 4 == 3) //Grass or Dirt?
							{
								if (GetRandomNumber(1, 40) == 5 && y < waterLevel - 4)
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(82)); //Clay
								else
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(12)); //Sand
							}
							if (y < waterLevel - 3)
								chunk.SetBlock(x, y + 1, z, BlockFactory.GetBlockById(8)); //Water
						}

						if (y <= dirtHeight && y >= stoneHeight)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(3)); //Dirt
							chunk.SetBlock(x, y + 1, z, BlockFactory.GetBlockById(2)); //Grass Block
							if (y > waterLevel)
							{
								//Grass
								if (GetRandomNumber(0, 5) == 2)
								{
									chunk.SetBlock(x, y + 2, z, new Block(31) {Metadata = 1});
								}

								//flower
								if (GetRandomNumber(0, 65) == 8)
								{
									var meta = GetRandomNumber(0, 8);
									chunk.SetBlock(x, y + 2, z, new Block(38) {Metadata = (ushort) meta});
								}

								for (var pos = 0; pos < trees; pos++)
								{
									if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 && treeBasePositions[pos, 1] < 14 &&
									    treeBasePositions[pos, 1] > 4)
									{
										if (y < waterLevel + 2)
											break;
										if (chunk.GetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]) >> 4 == 2)
										{
											if (y == dirtHeight)
												GenerateTree(chunk, treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]);
										}
									}
								}
							}
						}

						if (y == 0)
						{
							chunk.SetBlock(x, y, z, new BlockBedrock());
						}
					}
				}
			}

			WormMe(chunk);
		}

		private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z)
		{
			var treeheight = GetRandomNumber(4, 5);

			chunk.SetBlock(x, treebase + treeheight + 2, z, BlockFactory.GetBlockById(18)); //Top leave

			chunk.SetBlock(x, treebase + treeheight + 1, z + 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x, treebase + treeheight + 1, z - 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x + 1, treebase + treeheight + 1, z, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x - 1, treebase + treeheight + 1, z, BlockFactory.GetBlockById(18));

			chunk.SetBlock(x, treebase + treeheight, z + 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x, treebase + treeheight, z - 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x + 1, treebase + treeheight, z, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x - 1, treebase + treeheight, z, BlockFactory.GetBlockById(18));

			chunk.SetBlock(x + 1, treebase + treeheight, z + 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x - 1, treebase + treeheight, z - 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x + 1, treebase + treeheight, z - 1, BlockFactory.GetBlockById(18));
			chunk.SetBlock(x - 1, treebase + treeheight, z + 1, BlockFactory.GetBlockById(18));

			for (var i = 0; i <= treeheight; i++)
			{
				chunk.SetBlock(x, treebase + i, z, BlockFactory.GetBlockById(17));
			}
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			if (!ChunkCache.TryGetValue(new Tuple<int, int>(block.Coordinates.X/16, block.Coordinates.Z/16), out c))
				throw new Exception("No chunk found!");

			c.SetBlock((block.Coordinates.X & 0x0f), (block.Coordinates.Y & 0x7f), (block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

			foreach (var player in level.OnlinePlayers)
			{
				new BlockChange(player.Wrapper, new MSGBuffer(player.Wrapper))
				{
					Block = block,
					Location = block.Coordinates
				}.Write();
			}
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(1, 40, 1);
		}

		public static int GetNoise(int x, int z, float scale, int max, NoiseGenerator? gen = null)
		{
			switch (gen.HasValue ? gen.Value : Globals.NoiseGenerator)
			{
				case NoiseGenerator.Perlin:
					return (int) Math.Floor((PerlinNoise.Noise(x*scale, 0, z*scale) + 1f)*(max/2f));
				case NoiseGenerator.Simplex:
					return (int) Math.Floor((Noise.Generate(_seedoffset + x*scale, 0, _seedoffset + z*scale) + 1f)*(max/2f));
				case NoiseGenerator.OpenSimplex:
					return (int) Math.Floor((OpenNoise.Evaluate(x*scale, z*scale) + 1f)*(max/2f));
				default:
					return (int) Math.Floor((Noise.Generate(_seedoffset + x*scale, 0, _seedoffset + z*scale) + 1f)*(max/2f));
			}
		}

		public static int Get3DNoise(int x, int y, int z, float scale, int max, NoiseGenerator? gen = null)
		{
			switch (gen.HasValue ? gen.Value : Globals.NoiseGenerator)
			{
				case NoiseGenerator.Perlin:
					return (int) Math.Floor((PerlinNoise.Noise(x*scale, y*scale, z*scale) + 1f)*(max/2f));
				case NoiseGenerator.Simplex:
					return
						(int)
							Math.Floor((Noise.Generate(_seedoffset + x*scale, _seedoffset + y*scale, _seedoffset + z*scale) + 1f)*(max/2f));
				case NoiseGenerator.OpenSimplex:
					return (int) Math.Floor((OpenNoise.Evaluate(x*scale, y*scale, z*scale) + 1f)*(max/2f));
				default:
					return (int) Math.Floor((Noise.Generate(_seedoffset + x*scale, y*scale, _seedoffset + z*scale) + 1f)*(max/2f));
			}
		}

		public void WormMe(ChunkColumn chunk)
		{
			//Generate Caves
			//TODO!!!
		}
	}
}