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

namespace SharpMC.Worlds.Standard
{
	internal class StandardWorldProvider : IWorldProvider
	{
		public static int WaterLevel = 72;
		private static readonly Random Getrandom = new Random();
		private static readonly object SyncLock = new object();
		private readonly BiomeManager _biomeManager;
		private readonly CaveGenerator _cavegen = new CaveGenerator(Globals.Seed.GetHashCode());
		private readonly string _folder;
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public StandardWorldProvider(string folder)
		{
			_folder = folder;
			IsCaching = true;
			_biomeManager = new BiomeManager(Globals.Seed.GetHashCode());
			_biomeManager.AddBiomeType(new PlainsBiome());
			_biomeManager.AddBiomeType(new ForestBiome());
			_biomeManager.AddBiomeType(new DesertBiome());
			_biomeManager.AddBiomeType(new BirchForestBiome());
			_biomeManager.AddBiomeType(new FlowerForestBiome());
			_biomeManager.AddBiomeType(new SunFlowerPlainsBiome());
		}

		public override sealed bool IsCaching { get; set; }

		public override ChunkColumn GetChunk(int x, int z)
		{
			foreach (var ch in ChunkCache.ToArray())
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

			var metalength = reader.ReadInt();
			var blockmeta = reader.ReadUShortLocal(metalength);

			var blockies = new Block[block.Length];
			for (var i = 0; i < block.Length; i++)
			{
				blockies[i] = new Block(block[i]) {Metadata = (byte) blockmeta[i]};
			}
			var skyLength = reader.ReadInt();
			var skylight = reader.Read(skyLength);

			var lightLength = reader.ReadInt();
			var blocklight = reader.Read(lightLength);

			var biomeIdLength = reader.ReadInt();
			var biomeId = reader.Read(biomeIdLength);

			var cc = new ChunkColumn
			{
				Blocks = blockies,
				Blocklight = {Data = blocklight},
				Skylight = {Data = skylight},
				BiomeId = biomeId,
				X = x,
				Z = z
			};
			Debug.WriteLine("We should have loaded " + x + ", " + z);
			return cc;
			//throw new NotImplementedException();
		}

		public override void SaveChunks(string folder)
		{
			lock (ChunkCache)
			{
				foreach (var i in ChunkCache)
				{
					File.WriteAllBytes(_folder + "/" + i.Value.X + "." + i.Value.Z + ".cfile", Globals.Compress(i.Value.Export()));
				}
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

			if (!ChunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
				ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public override IEnumerable<ChunkColumn> GenerateChunks(int viewDistance, double playerX, double playerZ,
			Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, Player player, bool output = false)
		{
			lock (chunksUsed)
			{
				var newOrders = new Dictionary<Tuple<int, int>, double>();
				var radiusSquared = viewDistance/Math.PI;
				var radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				var centerX = (int) (playerX) >> 4;
				var centerZ = (int) (playerZ) >> 4;

				for (var x = -radius; x <= radius; ++x)
				{
					for (var z = -radius; z <= radius; ++z)
					{
						var distance = (x*x) + (z*z);
						if (distance > radiusSquared)
						{
							continue;
						}
						var chunkX = (int) (x + centerX);
						var chunkZ = (int) (z + centerZ);
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

				foreach (var pair in newOrders.OrderBy(pair => pair.Value))
				{
					if (chunksUsed.ContainsKey(pair.Key)) continue;

					var chunk = GenerateChunkColumn(new ChunkCoordinates(pair.Key.Item1, pair.Key.Item2));
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
				return Getrandom.Next(min, max);
			}
		}
		
		//World Tweaking settings
		//These settings can be tweaked to changed the look of the terrain.

		private const double OverhangOffset = 32.0; //Old value: 32.0 || Changes the offset from the bottom ground.
		private const double BottomOffset = 96.0; //Old value: 96.0  || Changes the offset from y level 0

		private const double OverhangsMagnitude = 16.0; //Old value: 16.0
		private const double BottomsMagnitude = 32.0; //Old value: 32.0

		private const double OverhangScale = 128.0; //Old value: 128.0 || Changes the scale of the overhang.
		private const double Groundscale = 256.0; //Old value: 256.0   || Changes the scale of the ground.

		private const double Threshold = 0.0; //Old value: 0.0 || Cool value: -0.3 hehehe

		private const bool EnableOverhang = true; //Enable overhang?

		private void PopulateChunk(ChunkColumn chunk)
		{
			var bottom = new SimplexOctaveGenerator(Globals.Seed.GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator(Globals.Seed.GetHashCode(), 8);
			overhang.SetScale(1/OverhangScale);
			bottom.SetScale(1/Groundscale);

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;

					var cBiome = _biomeManager.GetBiome((int) ox, (int) oz);
					chunk.BiomeId[x*16 + z] = cBiome.MinecraftBiomeId;

					var bottomHeight = (int) ((bottom.Noise(ox, oz, 0.5, 0.5)*BottomsMagnitude) + BottomOffset);
					var maxHeight = (int) ((overhang.Noise(ox, oz, 0.5, 0.5)*OverhangsMagnitude) + bottomHeight + OverhangOffset);

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
							//part where we do the overhangs
							if (EnableOverhang)
							{
								var density = overhang.Noise(ox, y, oz, 0.5, 0.5);
								if (density > Threshold) chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
							}
						}
						else
						{
							chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(1));
						}
					}

					//Turn the blocks ontop into the correct material
					for (int y = bottomHeight; y < 256; y++)
					{
						if (chunk.GetBlock(x, y + 1, z) == 0 && chunk.GetBlock(x,y,z) == 1)
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
					new OreDecorator().Decorate(chunk, cBiome, x, z);//Ores :)
					new BedrockDecorator().Decorate(chunk, cBiome, x, z); //Random bedrock :)
				}
			}

			new WaterDecorator().Decorate(chunk, new PlainsBiome()); //For now, ALWAYS use the water decorator on all chunks...
			_cavegen.GenerateCave(chunk);
			new LavaDecorator().Decorate(chunk, new PlainsBiome());
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			if (
				!ChunkCache.TryGetValue(new Tuple<int, int>((int) block.Coordinates.X >> 4, (int) block.Coordinates.Z >> 4), out c))
				throw new Exception("No chunk found!");

			c.SetBlock(((int) block.Coordinates.X & 0x0f), ((int) block.Coordinates.Y & 0x7f), ((int) block.Coordinates.Z & 0x0f),
				block);
			if (!broadcast) return;

			BlockChange.Broadcast(block);
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(0, 82, 0);
		}
	}
}