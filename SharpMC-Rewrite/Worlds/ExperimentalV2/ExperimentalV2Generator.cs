using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibNoise;
using LibNoise.Primitive;
using MiNET.Worlds;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Worlds.ExperimentalV2.Structures;

namespace SharpMCRewrite.Worlds.ExperimentalV2
{
	internal class SimplexOctaveGenerator
	{
		private readonly SimplexPerlin[] _generators;
		private readonly int _octaves;
		private readonly long _seed;

		public SimplexOctaveGenerator(int seed, int octaves)
		{
			_seed = seed;
			_octaves = octaves;

			_generators = new SimplexPerlin[octaves];
			for (var i = 0; i < _generators.Length; i++)
			{
				_generators[i] = new SimplexPerlin(seed, NoiseQuality.Fast);
			}
		}

		public double XScale { get; set; }
		public double YScale { get; set; }
		public double ZScale { get; set; }
		public double WScale { get; set; }

		public double Noise(double x, double y, double frequency, double amplitude)
		{
			return Noise(x, y, 0, 0, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double z, double frequency, double amplitude)
		{
			return Noise(x, y, z, 0, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double z, double w, double frequency, double amplitude)
		{
			return Noise(x, y, z, w, frequency, amplitude, false);
		}

		public double Noise(double x, double y, double z, double w, double frequency, double amplitude, bool normalized)
		{
			double result = 0;
			double amp = 1;
			double freq = 1;
			double max = 0;

			x *= XScale;
			y *= YScale;
			z *= ZScale;
			w *= WScale;

			foreach (var octave in _generators)
			{
				result += octave.GetValue((float) (x*freq), (float) (y*freq), (float) (z*freq), (float) (w*freq))*amp;
				max += amp;
				freq *= frequency;
				amp *= amplitude;
			}

			if (normalized)
			{
				result /= max;
			}

			return result;
		}

		public void SetScale(double scale)
		{
			XScale = scale;
			YScale = scale;
			ZScale = scale;
			WScale = scale;
		}
	}

	internal class ExperimentalV2Generator : IWorldProvider
	{
		private const int WaterLevel = 50;
		private const string _Seed = "Test";
		private static int _seedoffset = new Random(Globals.Seed.GetHashCode()).Next(1, Int16.MaxValue);
		private static readonly Random getrandom = new Random();
		private static readonly object syncLock = new object();
		private readonly string _folder = "";
		private readonly CaveGenerator cavegen = new CaveGenerator(_Seed.GetHashCode());
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public ExperimentalV2Generator(string folder)
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
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed)
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
			var trees = GetRandomNumber(0, 10);
			var treeBasePositions = new int[trees, 2];

			for (var t = 0; t < trees; t++)
			{
				var x = new Random().Next(1, 16);
				var z = new Random().Next(1, 16);
				treeBasePositions[t, 0] = x;
				treeBasePositions[t, 1] = z;
			}

			var bottom = new SimplexOctaveGenerator(_Seed.GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator(_Seed.GetHashCode(), 8);
			overhang.SetScale(1/128.0);
			bottom.SetScale(1/256.0);

			double overhangsMagnitude = 16;
			double bottomsMagnitude = 32;

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;


					var bottomHeight = (int) ((bottom.Noise(ox, oz, 0.5, 0.5)*bottomsMagnitude) + 64.0);
					var maxHeight = (int) ((overhang.Noise(ox, oz, 0.5, 0.5)*overhangsMagnitude) + bottomHeight + 32.0);

					var threshold = 0.0;

					maxHeight = Math.Max(1, maxHeight);

					for (var y = 0; y < maxHeight && y < 256; y++)
					{
						if (y <= 1)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(7));
							continue;
						}

						if (y > bottomHeight)
						{
							//part where we do the overhangs
							var density = overhang.Noise(ox, y, oz, 0.5, 0.5);
							if (density > threshold) chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
						}
						else
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
						}
					}

					//turn the tops into grass
					if (chunk.GetBlock(x, bottomHeight, z) != 18)
						chunk.SetBlock(x, bottomHeight, z, BlockFactory.GetBlockById(2)); //the top of the base hills

					chunk.SetBlock(x, bottomHeight - 1, z, BlockFactory.GetBlockById(3));
					chunk.SetBlock(x, bottomHeight - 2, z, BlockFactory.GetBlockById(3));


					for (var y = bottomHeight + 1; y > bottomHeight && y < maxHeight && y < 127; y++)
					{
						//the overhang
						var thisblock = chunk.GetBlock(x, y, z);
						var blockabove = chunk.GetBlock(x, y + 1, z);

						if (thisblock != 0 && blockabove == 0 && thisblock != 18 && blockabove != 18)
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(2));
							if (chunk.GetBlock(x, y - 1, z) != 0)
								chunk.SetBlock(x, y - 1, z, BlockFactory.GetBlockById(3));
							if (chunk.GetBlock(x, y - 2, z) != 0)
								chunk.SetBlock(x, y - 2, z, BlockFactory.GetBlockById(3));
						}
					}

					for (var y = 0; y < WaterLevel; y++)
					{
						//Lake generation
						if (y < WaterLevel)
						{
							if (chunk.GetBlock(x, y, z) == 2 || chunk.GetBlock(x, y, z) == 3) //Grass or Dirt?
							{
								if (GetRandomNumber(1, 40) == 1 && y < WaterLevel - 4)
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(82)); //Clay
								else
									chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(12)); //Sand
							}
							if (chunk.GetBlock(x, y + 1, z) == 0)
							{
								if (y < WaterLevel - 3)
									chunk.SetBlock(x, y + 1, z, BlockFactory.GetBlockById(8)); //Water
							}
						}
					}

					for (var y = 0; y < 256; y++)
					{
						var thisblock = chunk.GetBlock(x, y, z);
						var blockabove = chunk.GetBlock(x, y + 1, z);
						if (thisblock == 2 && blockabove == 0 && y > WaterLevel)
						{
							//Grass
							if (GetRandomNumber(0, 5) == 2)
							{
								chunk.SetBlock(x, y + 1, z, new Block(31) {Metadata = 1});
							}

							//Flowers
							if (GetRandomNumber(0, 65) == 8)
							{
								var meta = GetRandomNumber(0, 8);
								chunk.SetBlock(x, y + 1, z, new Block(38) {Metadata = (ushort) meta});
							}

							//Trees
							for (var pos = 0; pos < trees; pos++)
							{
								if (treeBasePositions[pos, 0] < 14 && treeBasePositions[pos, 0] > 4 && treeBasePositions[pos, 1] < 14 &&
								    treeBasePositions[pos, 1] > 4)
								{
									if (chunk.GetBlock(treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]) == 2)
									{
										if (y >= bottomHeight)
											GenerateTree(chunk, treeBasePositions[pos, 0], y + 1, treeBasePositions[pos, 1]);
									}
								}
							}
						}
						if (y < WaterLevel - 10)
						{
							cavegen.GenerateCave(chunk, x, y, z);
						}
					}
				}
			}
		}

		private void GenerateTree(ChunkColumn chunk, int x, int treebase, int z)
		{
			new OakTree().Create(chunk, x, treebase, z);
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			if (!ChunkCache.TryGetValue(new Tuple<int, int>(block.Coordinates.X >> 4, block.Coordinates.Z >> 4), out c))
				throw new Exception("No chunk found!");

			c.SetBlock((block.Coordinates.X & 0x0f), (block.Coordinates.Y & 0x7f), (block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

			foreach (var player in level.OnlinePlayers)
			{
				new BlockChange(player.Wrapper)
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
	}

	internal enum WoodType : byte
	{
		Oak = 0,
		Spruce = 1,
		Birch = 2,
		Jungle = 3
	}
}