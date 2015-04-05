using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LibNoise;
using LibNoise.Primitive;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Interfaces;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Worlds.ExperimentalV2.Structures;

namespace SharpMCRewrite.Worlds.ExperimentalV2
{
	internal class SimplexOctaveGenerator
	{
		private readonly SimplexPerlin[] _generators;
		public int Octaves { get; private set; }
		public long Seed { get; private set; }

		public SimplexOctaveGenerator(int seed, int octaves)
		{
			Seed = seed;
			Octaves = octaves;

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
		private const string Seed = "Testert";
		private static int _seedoffset = new Random(Globals.Seed.GetHashCode()).Next(1, Int16.MaxValue);
		private static readonly Random Getrandom = new Random();
		private static readonly object SyncLock = new object();
		private readonly string _folder;
		private readonly CaveGenerator _cavegen = new CaveGenerator(Seed.GetHashCode());
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public ExperimentalV2Generator(string folder)
		{
			_folder = folder;
			IsCaching = true;
		}

		public override sealed bool IsCaching { get; set; }

		public static int Seedoffset
		{
			get { return _seedoffset; }
			set { _seedoffset = value; }
		}

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

			var blockLength = reader.ReadInt();
			var block = reader.ReadUShortLocal(blockLength);

			var skyLength = reader.ReadInt();
			var skylight = reader.Read(skyLength);

			var lightLength = reader.ReadInt();
			var blocklight = reader.Read(lightLength);

			var biomeIdLength = reader.ReadInt();
			var biomeId = reader.Read(biomeIdLength);

			var cc = new ChunkColumn
			{
				Blocks = block,
				Blocklight = {Data = blocklight},
				Skylight = {Data = skylight},
				BiomeId = biomeId,
				X = x,
				Z = z
			};
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return cc;
		}

		public override void SaveChunks(string folder)
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

		public override IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, Player player, bool output = false)
		{
			lock (chunksUsed)
			{
				Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int, int>, double>();
				double radiusSquared = viewDistance / Math.PI;
				double radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				int centerX = (int)(playerX) >> 4;
				int centerZ = (int)(playerZ) >> 4;

				for (double x = -radius; x <= radius; ++x)
				{
					for (double z = -radius; z <= radius; ++z)
					{
						var distance = (x * x) + (z * z);
						if (distance > radiusSquared)
						{
							continue;
						}
						int chunkX = (int)(x + centerX);
						int chunkZ = (int)(z + centerZ);
						Tuple<int, int> index = new Tuple<int, int>(chunkX, chunkZ);
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
							Chunk = new ChunkColumn() { X = chunkKey.Item1, Z = chunkKey.Item2 }
						}.Write();

						chunksUsed.Remove(chunkKey);
					}
				}

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					ChunkColumn chunk = GenerateChunkColumn(new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2));
					chunksUsed.Add(pair.Key, chunk);

					yield return chunk;
				}

				if (chunksUsed.Count > viewDistance) Debug.WriteLine("Too many chunks used: {0}", chunksUsed.Count);
			}
		}

		public static int GetRandomNumber(int min, int max)
		{
			lock (SyncLock)
			{
				// synchronize
				return Getrandom.Next(min, max);
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

			var bottom = new SimplexOctaveGenerator(Seed.GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator(Seed.GetHashCode(), 8);
			overhang.SetScale(1/128.0);
			bottom.SetScale(1/256.0);

			const double overhangsMagnitude = 16;
			const double bottomsMagnitude = 32;

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;


					var bottomHeight = (int) ((bottom.Noise(ox, oz, 0.5, 0.5)*bottomsMagnitude) + 64.0);
					var maxHeight = (int) ((overhang.Noise(ox, oz, 0.5, 0.5)*overhangsMagnitude) + bottomHeight + 32.0);

					const double threshold = 0.0;

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
								if (GetRandomNumber(0, 50) == 10)
								{
									chunk.SetBlock(x, y + 1, z, new Block(175) {Metadata = 2});
									chunk.SetBlock(x, y + 2, z, new Block(175) {Metadata = 2});
								}
								else
								{
									chunk.SetBlock(x, y + 1, z, new Block(31) { Metadata = 1 });
								}
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
							_cavegen.GenerateCave(chunk, x, y, z);
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
			if (!ChunkCache.TryGetValue(new Tuple<int, int>((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4), out c))
				throw new Exception("No chunk found!");

			c.SetBlock(((int)block.Coordinates.X & 0x0f), ((int)block.Coordinates.Y & 0x7f), ((int)block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;
			//ChunkData.Broadcast(c); //Hehe, blockchanges are bugged, so temporaily solution :P
			BlockChange.Broadcast(block);
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