using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Interfaces;
using SharpMCRewrite.Networking.Packages;

namespace SharpMCRewrite.Worlds.Flatland
{
	public class FlatLandGenerator : IWorldProvider
	{
		private readonly string Folder = "world";
		public Dictionary<Tuple<int, int>, ChunkColumn> _chunkCache = new Dictionary<Tuple<int, int>, ChunkColumn>();

		public FlatLandGenerator(string folder)
		{
			Folder = folder;
			IsCaching = true;
		}

		public override bool IsCaching { get; set; }

		public override void Initialize()
		{
		}

		public override ChunkColumn GetChunk(int x, int z)
		{
			foreach (var ch in _chunkCache)
			{
				if (ch.Key.Item1 == x && ch.Key.Item2 == z)
				{
					return ch.Value;
				}
			}
			throw new Exception("We couldn't find the chunk.");
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
						// new Networking.Packages.ChunkData(wrapper, new MSGBuffer(wrapper))
						// {
						//     Chunk = new ChunkColumn() {X = chunkKey.Item1, Z = chunkKey.Item2}
						// }.Write(); //Unload the chunk client on the client side.

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

			_chunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

			return chunk;
		}

		public override Vector3 GetSpawnPoint()
		{
			return new Vector3(1, 1, 1);
		}

		public int PopulateChunk(ChunkColumn chunk)
		{
			var blocks = new ushort[16*16*256];
			var Last = 0;
			for (var x = 0; x < 256; x ++)
			{
				blocks[x] = (7 << 4) | 0; // Bedrock
				Last++;
			}

			for (var x = Last; x < (256*3); x ++)
			{
				blocks[x] = (3 << 4) | 0; // Dirt?
				Last++;
			}

			for (var x = Last; x < (256*4); x ++)
			{
				blocks[x] = (2 << 4) | 0; // Grass??
				Last++;
			}

			chunk.Blocks = blocks;
			return 4;
		}

		public override void SaveChunks(string folder)
		{
			foreach (var i in _chunkCache)
			{
				File.WriteAllBytes(folder + "/" + i.Value.X + "." + i.Value.Z + ".cfile", Globals.Compress(i.Value.Export()));
			}
		}

		public override ChunkColumn LoadChunk(int x, int z)
		{
			var u = Globals.Decompress(File.ReadAllBytes(Folder + "/" + x + "." + z + ".cfile"));
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

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			if (!_chunkCache.TryGetValue(new Tuple<int, int>((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4), out c))
				throw new Exception("No chunk found!");

			c.SetBlock(((int)block.Coordinates.X & 0x0f), ((int)block.Coordinates.Y & 0x7f), ((int)block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

			foreach (var player in level.OnlinePlayers)
			{
				new BlockChange(player.Wrapper, new MSGBuffer(player.Wrapper))
				{
					BlockId = block.Id,
					MetaData = block.Metadata,
					Location = block.Coordinates
				}.Write();
			}
		}
	}
}