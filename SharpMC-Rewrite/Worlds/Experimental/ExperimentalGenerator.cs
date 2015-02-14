using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccidentalNoise;
using MiNET.Worlds;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Networking;
using SimplexNoise;
using System.IO;

namespace SharpMCRewrite.Worlds.Experimental
{
	class ExperimentalGenerator : IWorldProvider
	{
		float stoneBaseHeight = 0;
		float stoneBaseNoise = 0.05f;
		float stoneBaseNoiseHeight = 4;

		float stoneMountainHeight = 48;
		float stoneMountainFrequency = 0.008f;
		float stoneMinHeight = 0;

		float dirtBaseHeight = 1;
		float dirtNoise = 0.04f;
		float dirtNoiseHeight = 3;

		private string _folder = "";
		public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();
		public override bool IsCaching { get; set; }

		public ExperimentalGenerator(string folder)
        {
            _folder = folder;
            IsCaching = true;
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
			byte[] u = Globals.Decompress(File.ReadAllBytes(_folder + "/" + x + "." + z + ".cfile"));
			MSGBuffer reader = new MSGBuffer(u);

			int BlockLength = reader.ReadInt();
			ushort[] Block = reader.ReadUShortLocal(BlockLength);

			int SkyLength = reader.ReadInt();
			byte[] Skylight = reader.Read(SkyLength);

			int LightLength = reader.ReadInt();
			byte[] Blocklight = reader.Read(LightLength);

			int BiomeIDLength = reader.ReadInt();
			byte[] BiomeID = reader.Read(BiomeIDLength);

			ChunkColumn CC = new ChunkColumn();
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
				ChunkColumn cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
				if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
					ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
				return cd;
			}

			Debug.WriteLine("ChunkFile not found, generating...");

			var chunk = new ChunkColumn { X = chunkCoordinates.X, Z = chunkCoordinates.Z };
			PopulateChunk(chunk);

			ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public override IEnumerable<ChunkColumn> GenerateChunks(int _viewDistance, double playerX, double playerZ, Dictionary<Tuple<int, int>, ChunkColumn> chunksUsed, ClientWrapper wrapper)
		{
			lock (chunksUsed)
			{
				Dictionary<Tuple<int, int>, double> newOrders = new Dictionary<Tuple<int, int>, double>();
				double radiusSquared = _viewDistance / Math.PI;
				double radius = Math.Ceiling(Math.Sqrt(radiusSquared));
				double centerX = Math.Floor((playerX) / 16);
				double centerZ = Math.Floor((playerZ) / 16);

				for (double x = -radius; x <= radius; ++x)
				{
					for (double z = -radius; z <= radius; ++z)
					{
						var distance = (x * x) + (z * z);
						if (distance > radiusSquared)
						{
							continue;
						}
						int chunkX = (int)Math.Floor(x + centerX);
						int chunkZ = (int)Math.Floor(z + centerZ);

						Tuple<int, int> index = new Tuple<int, int>((int)chunkX, (int)chunkZ);
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

					int x = pair.Key.Item1;
					int z = pair.Key.Item2;

					ChunkColumn chunk = GenerateChunkColumn(new Vector2(x, z));
					chunksUsed.Add(pair.Key, chunk);

					yield return chunk;
				}
			}
		}


		private static Random rnd = new Random("SharpMCSeed".GetHashCode());
		private static PerlinNoise hillsNoise = new PerlinNoise(rnd.Next());
		private bool waterleveldefined = false;
		private int waterlevel = 25;

		private void PopulateChunk(ChunkColumn chunk)
		{
			int trees = new Random().Next(0, 10);
			int[,] treeBasePositions = new int[trees, 2];

			int chunkX = chunk.X*16;
			int chunkZ = chunk.Z*16;

			for (int x = chunkX; x < chunkX + 16; x++)
			{
				for (int z = chunkZ; z < chunkZ + 16; z++)
				{
					int stoneHeight = (int) Math.Floor(stoneBaseHeight);
					stoneHeight += GetNoise(x, z, stoneMountainFrequency, (int)Math.Floor(stoneMountainHeight));

					if (stoneHeight < stoneMinHeight)
						stoneHeight = (int) Math.Floor(stoneMinHeight);

					stoneHeight += GetNoise(x, z, stoneBaseNoise, (int) Math.Floor(stoneBaseNoiseHeight));

					int dirtHeight = stoneHeight + (int) Math.Floor(dirtBaseHeight);
					dirtHeight += GetNoise(x, z, dirtNoise, (int) Math.Floor(dirtNoiseHeight));

					for (int y = 0; y < 256; y++)
					{
							if (y == 0)
							{
								chunk.SetBlock(x,y,z, new BlockBedrock());
							}
							else if (y <= stoneHeight)
							{
								chunk.SetBlock(x , y, z, BlockFactory.GetBlockById(1));
							}
							else if (y <= dirtHeight)
							{
								chunk.SetBlock(x, y, z, BlockFactory.GetBlockById(3));
								chunk.SetBlock(x, y + 1, z, BlockFactory.GetBlockById(2));
							}
					}
				}
			}
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			if (!ChunkCache.TryGetValue(new Tuple<int, int>(block.Coordinates.X / 16, block.Coordinates.Z / 16), out c)) throw new Exception("No chunk found!");

			c.SetBlock((block.Coordinates.X & 0x0f), (block.Coordinates.Y & 0x7f), (block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

			foreach (var player in level.OnlinePlayers)
			{
				new Networking.Packages.BlockChange(player.Wrapper, new MSGBuffer(player.Wrapper))
				{
					Block = block,
					Location = block.Coordinates
				}.Write();
			}
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(1,1,1);
		}

		public static int GetNoise(int x, int z, float scale, int max, bool perlin = true)
		{
			if (perlin)
				return (int)Math.Floor((hillsNoise.Noise(x * scale, -3, z * scale) + 1f) * (max / 2f));
			else
				return (int)Math.Floor((SimplexNoise.Noise.Generate(x * scale, scale, z * scale) + 1f) * (max / 2f));
		}
	}
}
