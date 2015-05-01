using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using fNbt;
using SharpMC.Blocks;
using SharpMC.Entity;
using SharpMC.Interfaces;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds.Standard;

namespace SharpMC.Worlds.Anvil
{
	public class AnvilWorldProvider : IWorldProvider
	{
		private readonly StandardWorldProvider _backEndGenerator;

		private readonly ConcurrentDictionary<ChunkCoordinates, ChunkColumn> _chunkCache =
			new ConcurrentDictionary<ChunkCoordinates, ChunkColumn>();

		private string _basePath;
		private LevelInfo _level;
		//private static readonly ILog Log = LogManager.GetLogger(typeof(AnvilWorldProvider));
		private byte _waterOffsetY;

		public AnvilWorldProvider()
		{
			IsCaching = true;
			_backEndGenerator = new StandardWorldProvider("v2");
		}

		public AnvilWorldProvider(string basePath)
			: this()
		{
			_basePath = basePath;
			Init();
		}

		public override sealed bool IsCaching { get; set; }

		public void Init()
		{
			_basePath = _basePath ?? Config.GetProperty("PCWorldFolder", "World").Trim();

			var file = new NbtFile();
			file.LoadFromFile(Path.Combine(_basePath, "level.dat"));
			var dataTag = file.RootTag["Data"];
			_level = new LevelInfo(dataTag);

			_waterOffsetY = (byte) Config.GetProperty("PCWaterOffset", 0);
		}

		private byte Nibble4(byte[] arr, int index)
		{
			return (byte) (index%2 == 0 ? arr[index/2] & 0x0F : (arr[index/2] >> 4) & 0x0F);
		}

		private static void SetNibble4(byte[] arr, int index, byte value)
		{
			if (index%2 == 0)
			{
				arr[index/2] = (byte) ((value & 0x0F) | arr[index/2]);
			}
			else
			{
				arr[index/2] = (byte) (((value << 4) & 0xF0) | arr[index/2]);
			}
		}

		public static void SaveChunk(ChunkColumn chunk, string basePath, int yoffset)
		{
			var coordinates = new ChunkCoordinates(chunk.X, chunk.Z);

			var width = 32;
			var depth = 32;

			var rx = coordinates.X >> 5;
			var rz = coordinates.Z >> 5;

			var filePath = Path.Combine(basePath, string.Format(@"region\r.{0}.{1}.mca", rx, rz));

			if (!File.Exists(filePath))
			{
				// Make sure directory exist
				Directory.CreateDirectory(Path.Combine(basePath, "region"));

				// Create empty region file
				using (var regionFile = File.Open(filePath, FileMode.CreateNew))
				{
					var buffer = new byte[8192];
					regionFile.Write(buffer, 0, buffer.Length);
				}

				return;
			}

			using (var regionFile = File.Open(filePath, FileMode.Open))
			{
				var buffer = new byte[8192];
				regionFile.Read(buffer, 0, buffer.Length);

				var xi = (coordinates.X%width);
				if (xi < 0) xi += 32;
				var zi = (coordinates.Z%depth);
				if (zi < 0) zi += 32;
				var tableOffset = (xi + zi*width)*4;

				regionFile.Seek(tableOffset, SeekOrigin.Begin);

				var offsetBuffer = new byte[4];
				regionFile.Read(offsetBuffer, 0, 3);
				Array.Reverse(offsetBuffer);
				var offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

				var length = regionFile.ReadByte();

				if (offset == 0 || length == 0)
				{
					regionFile.Seek(0, SeekOrigin.End);
					offset = (int) regionFile.Position;

					regionFile.Seek(tableOffset, SeekOrigin.Begin);

					var bytes = BitConverter.GetBytes(offset >> 4);
					Array.Reverse(bytes);
					regionFile.Write(bytes, 0, 3);
					regionFile.WriteByte(1);
				}

				regionFile.Seek(offset, SeekOrigin.Begin);
				var waste = new byte[4];
				regionFile.Write(waste, 0, 4); // Lenght
				regionFile.WriteByte(0x02); // Compression mode

				// Write NBT
				var nbt = CreateNbtFromChunkColumn(chunk, yoffset);
				//nbt.SaveToStream(regionFile, NbtCompression.ZLib);
				var nbtBuf = nbt.SaveToBuffer(NbtCompression.ZLib);
				regionFile.Write(nbtBuf, 0, nbtBuf.Length);

				var lenght = nbtBuf.Length + 5;
				int reminder;
				Math.DivRem(lenght, 4096, out reminder);

				var padding = new byte[4096 - reminder];
				if (padding.Length > 0) regionFile.Write(padding, 0, padding.Length);
			}
		}

		private static NbtFile CreateNbtFromChunkColumn(ChunkColumn chunk, int yoffset)
		{
			var nbt = new NbtFile();

			var levelTag = new NbtCompound("Level");
			nbt.RootTag.Add(levelTag);

			levelTag.Add(new NbtInt("xPos", chunk.X));
			levelTag.Add(new NbtInt("zPos", chunk.Z));
			levelTag.Add(new NbtByteArray("Biomes", chunk.BiomeId));

			var sectionsTag = new NbtList("Sections");
			levelTag.Add(sectionsTag);

			for (var i = 0; i < 8; i++)
			{
				var sectionTag = new NbtCompound();
				sectionsTag.Add(sectionTag);
				sectionTag.Add(new NbtByte("Y", (byte) i));
				var sy = i*16;

				var blocks = new byte[4096];
				var data = new byte[2048];
				var blockLight = new byte[2048];
				var skyLight = new byte[2048];

				for (var x = 0; x < 16; x++)
				{
					for (var z = 0; z < 16; z++)
					{
						for (var y = 0; y < 16; y++)
						{
							var yi = sy + y;
							if (yi < 0 || yi >= 256) continue; // ?

							var anvilIndex = (y + yoffset)*16*16 + z*16 + x;
							var blockId = chunk.GetBlock(x, yi, z);

							// PE to Anvil friendly converstion
							if (blockId == 5) blockId = 125;
							else if (blockId == 158) blockId = 126;
							else if (blockId == 50) blockId = 75;
							else if (blockId == 50) blockId = 76;
							else if (blockId == 89) blockId = 123;
							else if (blockId == 89) blockId = 124;
							else if (blockId == 73) blockId = 152;

							blocks[anvilIndex] = (byte) blockId;
							SetNibble4(data, anvilIndex, chunk.GetMetadata(x, yi, z));
							SetNibble4(blockLight, anvilIndex, chunk.GetBlocklight(x, yi, z));
							SetNibble4(skyLight, anvilIndex, chunk.GetSkylight(x, yi, z));
						}
					}
				}

				sectionTag.Add(new NbtByteArray("Blocks", blocks));
				sectionTag.Add(new NbtByteArray("Data", data));
				sectionTag.Add(new NbtByteArray("BlockLight", blockLight));
				sectionTag.Add(new NbtByteArray("SkyLight", skyLight));
			}

			levelTag.Add(new NbtList("Entities", NbtTagType.Compound));
			levelTag.Add(new NbtList("TileEntities", NbtTagType.Compound));
			levelTag.Add(new NbtList("TileTicks", NbtTagType.Compound));

			return nbt;
		}

		public int NumberOfCachedChunks()
		{
			return _chunkCache.Count;
		}

		public override ChunkColumn GenerateChunkColumn(Vector2 chunkCoordinates)
		{
			lock (_chunkCache)
			{
				ChunkColumn cachedChunk;
				if (_chunkCache.TryGetValue(new ChunkCoordinates(chunkCoordinates.X, chunkCoordinates.Z), out cachedChunk))
					return cachedChunk;

				var chunk = GetChunk(chunkCoordinates.X, chunkCoordinates.Z);

				_chunkCache[new ChunkCoordinates(chunkCoordinates.X, chunkCoordinates.Z)] = chunk;

				return chunk;
			}
		}

		public ChunkColumn GetChunk(int X, int Z)
		{
			var width = 32;
			var depth = 32;

			var rx = X >> 5;
			var rz = Z >> 5;

			var filePath = Path.Combine(_basePath, string.Format(@"region\r.{0}.{1}.mca", rx, rz));

			if (!File.Exists(filePath)) return _backEndGenerator.GenerateChunkColumn(new Vector2(X, Z));

			using (var regionFile = File.OpenRead(filePath))
			{
				var buffer = new byte[8192];

				regionFile.Read(buffer, 0, 8192);

				var xi = (X%width);
				if (xi < 0) xi += 32;
				var zi = (Z%depth);
				if (zi < 0) zi += 32;
				var tableOffset = (xi + zi*width)*4;

				regionFile.Seek(tableOffset, SeekOrigin.Begin);

				var offsetBuffer = new byte[4];
				regionFile.Read(offsetBuffer, 0, 3);
				Array.Reverse(offsetBuffer);
				var offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

				var length = regionFile.ReadByte();

				if (offset == 0 || length == 0) return _backEndGenerator.GenerateChunkColumn(new Vector2(X, Z));

				regionFile.Seek(offset, SeekOrigin.Begin);
				var waste = new byte[4];
				regionFile.Read(waste, 0, 4);
				var compressionMode = regionFile.ReadByte();

				var nbt = new NbtFile();
				nbt.LoadFromStream(regionFile, NbtCompression.ZLib);

				var dataTag = nbt.RootTag["Level"];

				var sections = dataTag["Sections"] as NbtList;

				var chunk = new ChunkColumn
				{
					X = X,
					Z = Z,
					BiomeId = dataTag["Biomes"].ByteArrayValue
				};

				for (var i = 0; i < chunk.BiomeId.Length; i++)
				{
					if (chunk.BiomeId[i] > 22) chunk.BiomeId[i] = 0;
				}
				if (chunk.BiomeId.Length > 256) throw new Exception();

				// This will turn into a full chunk column
				foreach (var sectionTag in sections)
				{
					var sy = sectionTag["Y"].ByteValue*16;
					var blocks = sectionTag["Blocks"].ByteArrayValue;
					var data = sectionTag["Data"].ByteArrayValue;
					var addTag = sectionTag["Add"];
					var adddata = new byte[2048];
					if (addTag != null) adddata = addTag.ByteArrayValue;
					var blockLight = sectionTag["BlockLight"].ByteArrayValue;
					var skyLight = sectionTag["SkyLight"].ByteArrayValue;

					for (var x = 0; x < 16; x++)
					{
						for (var z = 0; z < 16; z++)
						{
							for (var y = 0; y < 16; y++)
							{
								var yi = sy + y - _waterOffsetY;
								if (yi < 0 || yi >= 256) continue;

								var anvilIndex = y*16*16 + z*16 + x;
								var blockId = blocks[anvilIndex] + (Nibble4(adddata, anvilIndex) << 8);


								var b = BlockFactory.GetBlockById((ushort) blockId);
								b.Metadata = Nibble4(data, anvilIndex);
								chunk.SetBlock(x, yi, z, b);
								chunk.SetBlocklight(x, yi, z, Nibble4(blockLight, anvilIndex));
								chunk.SetSkylight(x, yi, z, Nibble4(skyLight, anvilIndex));
							}
						}
					}
				}

				var entities = dataTag["Entities"] as NbtList;
				var blockEntities = dataTag["TileEntities"] as NbtList;
				var tileTicks = dataTag["TileTicks"] as NbtList;

				chunk.IsDirty = false;
				return chunk;
			}
		}


		public override Vector3 GetSpawnPoint()
		{
			var spawnPoint = new Vector3(_level.SpawnX, _level.SpawnY, _level.SpawnZ);
			spawnPoint.Y += 2; // Compensate for point being at head
			spawnPoint.Y += _waterOffsetY; // Compensate for offset
			if (spawnPoint.Y > 256) spawnPoint.Y = 256;
			return spawnPoint;
		}

		public override void SaveChunks(string s)
		{
			lock (_chunkCache)
			{
				foreach (var chunkColumn in _chunkCache)
				{
					if (chunkColumn.Value.IsDirty) SaveChunk(chunkColumn.Value, _basePath, _waterOffsetY);
				}
			}
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

		public override ChunkColumn LoadChunk(int x, int y)
		{
			return GetChunk(x, y);
		}
	}
}