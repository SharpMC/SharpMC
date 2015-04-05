using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using fNbt;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Interfaces;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Utils;

namespace SharpMCRewrite.Worlds
{
	public class AnvilWorldProvider : IWorldProvider
	{
		//private static readonly ILog Log = LogManager.GetLogger(typeof(AnvilWorldProvider));
		private byte _waterOffsetY;
		private ExperimentalV2.ExperimentalV2Generator _flatland;
		private LevelInfo _level;
		private readonly ConcurrentDictionary<ChunkCoordinates, ChunkColumn> _chunkCache = new ConcurrentDictionary<ChunkCoordinates, ChunkColumn>();
		private string _basePath;

		public bool IsCaching { get; private set; }


		public AnvilWorldProvider()
		{
			IsCaching = true;
			_flatland = new ExperimentalV2.ExperimentalV2Generator("v2");
		}

		public AnvilWorldProvider(string basePath)
			: this()
		{
			_basePath = basePath;
			Init();
		}

		public void Init()
		{
			_basePath = _basePath ?? Config.GetProperty("PCWorldFolder", "World").Trim();

			NbtFile file = new NbtFile();
			file.LoadFromFile(Path.Combine(_basePath, "level.dat"));
			NbtTag dataTag = file.RootTag["Data"];
			_level = new LevelInfo(dataTag);

			_waterOffsetY = (byte)Config.GetProperty("PCWaterOffset", 0);
		}

		private byte Nibble4(byte[] arr, int index)
		{
			return (byte)(index % 2 == 0 ? arr[index / 2] & 0x0F : (arr[index / 2] >> 4) & 0x0F);
		}

		private void SetNibble4(byte[] arr, int index, byte value)
		{
			if (index % 2 == 0)
			{
				arr[index / 2] = (byte)((value & 0x0F) | arr[index / 2]);
			}
			else
			{
				arr[index / 2] = (byte)(((value << 4) & 0xF0) | arr[index / 2]);
			}
		}

		private void SaveChunks(ChunkColumn chunk)
		{
			var coordinates = new ChunkCoordinates(chunk.X, chunk.Z);

			int width = 32;
			int depth = 32;

			int rx = coordinates.X >> 5;
			int rz = coordinates.Z >> 5;

			string filePath = Path.Combine(_basePath, string.Format(@"region\r.{0}.{1}.mca", rx, rz));

			if (!File.Exists(filePath)) return;

			using (var regionFile = File.Open(filePath, FileMode.Open))
			{
				byte[] buffer = new byte[8192];
				regionFile.Read(buffer, 0, 8192);

				int xi = (coordinates.X % width);
				if (xi < 0) xi += 32;
				int zi = (coordinates.Z % depth);
				if (zi < 0) zi += 32;
				int tableOffset = (xi + zi * width) * 4;

				regionFile.Seek(tableOffset, SeekOrigin.Begin);

				byte[] offsetBuffer = new byte[4];
				regionFile.Read(offsetBuffer, 0, 3);
				Array.Reverse(offsetBuffer);
				int offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

				int length = regionFile.ReadByte();

				if (offset == 0 || length == 0)
				{
					//throw new Exception("New chunk");
					// New chunk, will need to append, but for now ignore
					return;
				}

				regionFile.Seek(offset, SeekOrigin.Begin);
				byte[] waste = new byte[4];
				regionFile.Read(waste, 0, 4);
				int compressionMode = regionFile.ReadByte();

				// Write NBT
				NbtFile nbt = CreateNbtFromChunkColumn(chunk);
				nbt.SaveToStream(regionFile, NbtCompression.ZLib);
			}
		}

		private NbtFile CreateNbtFromChunkColumn(ChunkColumn chunk)
		{
			var nbt = new NbtFile();

			NbtCompound levelTag = new NbtCompound("Level");
			nbt.RootTag.Add(levelTag);

			levelTag.Add(new NbtInt("xPos", chunk.X));
			levelTag.Add(new NbtInt("zPos", chunk.Z));
			levelTag.Add(new NbtByteArray("Biomes", chunk.biomeId));

			NbtList sectionsTag = new NbtList("Sections");
			levelTag.Add(sectionsTag);

			for (int i = 0; i < 8; i++)
			{
				NbtCompound sectionTag = new NbtCompound();
				sectionsTag.Add(sectionTag);
				sectionTag.Add(new NbtByte("Y", (byte)i));
				int sy = i * 16;

				byte[] blocks = new byte[4096];
				byte[] data = new byte[2048];
				byte[] blockLight = new byte[2048];
				byte[] skyLight = new byte[2048];

				for (int x = 0; x < 16; x++)
				{
					for (int z = 0; z < 16; z++)
					{
						for (int y = 0; y < 16; y++)
						{
							int yi = sy + y;
							if (yi < 0 || yi >= 256) continue; // ?

							int anvilIndex = (y + _waterOffsetY) * 16 * 16 + z * 16 + x;
							byte blockId = (byte)chunk.GetBlock(x, yi, z);

							blocks[anvilIndex] = blockId;
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
				if (_chunkCache.TryGetValue(new ChunkCoordinates(chunkCoordinates.X, chunkCoordinates.Z), out cachedChunk)) return cachedChunk;

				ChunkColumn chunk = GetChunk(chunkCoordinates.X, chunkCoordinates.Z);

				_chunkCache[new ChunkCoordinates(chunkCoordinates.X, chunkCoordinates.Z)] = chunk;

				return chunk;
			}
		}

		public override ChunkColumn GetChunk(int X, int Z)
		{
			lock (_chunkCache)
			{
				if (_chunkCache.ContainsKey(new ChunkCoordinates(X, Z)))
				{
					ChunkColumn cc;
					_chunkCache.TryGetValue(new ChunkCoordinates(X, Z), out cc);
					return cc;
				}
			}
			int width = 32;
			int depth = 32;

			int rx = X >> 5;
			int rz = Z >> 5;

			string filePath = Path.Combine(_basePath, string.Format(@"region\r.{0}.{1}.mca", rx, rz));

			if (!File.Exists(filePath)) return _flatland.GenerateChunkColumn(new Vector2(X, Z));

			using (var regionFile = File.OpenRead(filePath))
			{
				byte[] buffer = new byte[8192];

				regionFile.Read(buffer, 0, 8192);

				int xi = (X % width);
				if (xi < 0) xi += 32;
				int zi = (Z % depth);
				if (zi < 0) zi += 32;
				int tableOffset = (xi + zi * width) * 4;

				regionFile.Seek(tableOffset, SeekOrigin.Begin);

				byte[] offsetBuffer = new byte[4];
				regionFile.Read(offsetBuffer, 0, 3);
				Array.Reverse(offsetBuffer);
				int offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

				int length = regionFile.ReadByte();

				if (offset == 0 || length == 0) return _flatland.GenerateChunkColumn(new Vector2(X, Z));

				regionFile.Seek(offset, SeekOrigin.Begin);
				byte[] waste = new byte[4];
				regionFile.Read(waste, 0, 4);
				int compressionMode = regionFile.ReadByte();

				var nbt = new NbtFile();
				nbt.LoadFromStream(regionFile, NbtCompression.ZLib);

				NbtTag dataTag = nbt.RootTag["Level"];

				NbtList sections = dataTag["Sections"] as NbtList;

				ChunkColumn chunk = new ChunkColumn
				{
					X = X,
					Z = Z,
					biomeId = dataTag["Biomes"].ByteArrayValue
				};

				for (int i = 0; i < chunk.biomeId.Length; i++)
				{
					if (chunk.biomeId[i] > 22) chunk.biomeId[i] = 0;
				}
				if (chunk.biomeId.Length > 256) throw new Exception();

				// This will turn into a full chunk column
				foreach (NbtTag sectionTag in sections)
				{
					int sy = sectionTag["Y"].ByteValue * 16;
					byte[] blocks = sectionTag["Blocks"].ByteArrayValue;
					byte[] data = sectionTag["Data"].ByteArrayValue;
					NbtTag addTag = sectionTag["Add"];
					byte[] adddata = new byte[2048];
					if (addTag != null) adddata = addTag.ByteArrayValue;
					byte[] blockLight = sectionTag["BlockLight"].ByteArrayValue;
					byte[] skyLight = sectionTag["SkyLight"].ByteArrayValue;

					for (int x = 0; x < 16; x++)
					{
						for (int z = 0; z < 16; z++)
						{
							for (int y = 0; y < 16; y++)
							{
								int yi = sy + y - _waterOffsetY;
								if (yi < 0 || yi >= 128) continue;

								int anvilIndex = y * 16 * 16 + z * 16 + x;
								int blockId = blocks[anvilIndex] + (Nibble4(adddata, anvilIndex) << 8);


								Block b = BlockFactory.GetBlockById((ushort)blockId);
								b.Metadata = Nibble4(data, anvilIndex);
								chunk.SetBlock(x, yi, z, b);
								chunk.SetBlocklight(x, yi, z, Nibble4(blockLight, anvilIndex));
								chunk.SetSkylight(x, yi, z, Nibble4(skyLight, anvilIndex));
							}
						}
					}
				}

				NbtList entities = dataTag["Entities"] as NbtList;
				NbtList blockEntities = dataTag["TileEntities"] as NbtList;
				NbtList tileTicks = dataTag["TileTicks"] as NbtList;

				chunk.isDirty = false;
				lock (_chunkCache)
				{
					_chunkCache.GetOrAdd(new ChunkCoordinates(X, Z), chunk);
				}
				return chunk;
			}
		}

		public override void SetBlock(Block block, Level level, bool broadcast)
		{
			ChunkColumn c;
			lock (_chunkCache)
			{
				if (
					!_chunkCache.TryGetValue(new ChunkCoordinates((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4),
						out c))
					return;
			}

			c.SetBlock(((int)block.Coordinates.X & 0x0f), ((int)block.Coordinates.Y & 0x7f), ((int)block.Coordinates.Z & 0x0f), block);
			if (!broadcast) return;

			BlockChange.Broadcast(block);
		}


		public override Vector3 GetSpawnPoint()
		{
			var spawnPoint = new Vector3(_level.SpawnX, _level.SpawnY, _level.SpawnZ);
			spawnPoint.Y += 2; // Compensate for point being at head
			spawnPoint.Y += _waterOffsetY; // Compensate for offset
			if (spawnPoint.Y > 256) spawnPoint.Y = 256;
			return spawnPoint;
		}

		public override void SaveChunks(string folder)
		{
			lock (_chunkCache)
			{
				foreach (var chunkColumn in _chunkCache)
				{
					if (chunkColumn.Value.isDirty) SaveChunks(chunkColumn.Value);
				}
			}
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

		public override ChunkColumn LoadChunk(int x, int y)
		{
			return GetChunk(x, y);
		}
	}

	public class LevelInfo
	{
		public LevelInfo()
		{
		}

		public LevelInfo(NbtTag dataTag)
		{
			LoadFromNbt(dataTag);
		}

		public int Version { get; private set; }
		public bool Initialized { get; private set; }
		public string LevelName { get; set; }
		public string GeneratorName { get; set; }
		public int GeneratorVersion { get; set; }
		public string GeneratorOptions { get; set; }
		public long RandomSeed { get; set; }
		public bool MapFeatures { get; set; }
		public long LastPlayed { get; set; }
		public bool AllowCommands { get; set; }
		public bool Hardcore { get; set; }
		private int GameType { get; set; }
		public long Time { get; set; }
		public long DayTime { get; set; }
		public int SpawnX { get; set; }
		public int SpawnY { get; set; }
		public int SpawnZ { get; set; }
		public bool Raining { get; set; }
		public int RainTime { get; set; }
		public bool Thundering { get; set; }
		public int ThunderTime { get; set; }

		public T GetPropertyValue<T>(NbtTag tag, Expression<Func<T>> property)
		{
			var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
			if (propertyInfo == null)
			{
				throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
			}

			NbtTag nbtTag = tag[propertyInfo.Name];
			if (nbtTag == null)
			{
				nbtTag = tag[LowercaseFirst(propertyInfo.Name)];
			}

			if (nbtTag == null) return default(T);

			var mex = property.Body as MemberExpression;
			var target = Expression.Lambda(mex.Expression).Compile().DynamicInvoke();

			switch (nbtTag.TagType)
			{
				case NbtTagType.Unknown:
					break;
				case NbtTagType.End:
					break;
				case NbtTagType.Byte:
					if (propertyInfo.PropertyType == typeof(bool)) propertyInfo.SetValue(target, nbtTag.ByteValue == 1);
					else propertyInfo.SetValue(target, nbtTag.ByteValue);
					break;
				case NbtTagType.Short:
					propertyInfo.SetValue(target, nbtTag.ShortValue);
					break;
				case NbtTagType.Int:
					if (propertyInfo.PropertyType == typeof(bool)) propertyInfo.SetValue(target, nbtTag.IntValue == 1);
					else propertyInfo.SetValue(target, nbtTag.IntValue);
					break;
				case NbtTagType.Long:
					propertyInfo.SetValue(target, nbtTag.LongValue);
					break;
				case NbtTagType.Float:
					propertyInfo.SetValue(target, nbtTag.FloatValue);
					break;
				case NbtTagType.Double:
					propertyInfo.SetValue(target, nbtTag.DoubleValue);
					break;
				case NbtTagType.ByteArray:
					propertyInfo.SetValue(target, nbtTag.ByteArrayValue);
					break;
				case NbtTagType.String:
					propertyInfo.SetValue(target, nbtTag.StringValue);
					break;
				case NbtTagType.List:
					break;
				case NbtTagType.Compound:
					break;
				case NbtTagType.IntArray:
					propertyInfo.SetValue(target, nbtTag.IntArrayValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return (T)propertyInfo.GetValue(target);
		}

		public T SetPropertyValue<T>(NbtTag tag, Expression<Func<T>> property, bool upperFirst = true)
		{
			var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
			if (propertyInfo == null)
			{
				throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
			}

			NbtTag nbtTag = tag[propertyInfo.Name];
			if (nbtTag == null)
			{
				nbtTag = tag[LowercaseFirst(propertyInfo.Name)];
			}

			if (nbtTag == null) return default(T);

			var mex = property.Body as MemberExpression;
			var target = Expression.Lambda(mex.Expression).Compile().DynamicInvoke();

			switch (nbtTag.TagType)
			{
				case NbtTagType.Unknown:
					break;
				case NbtTagType.End:
					break;
				case NbtTagType.Byte:
					if (propertyInfo.PropertyType == typeof(bool))
						tag[nbtTag.Name] = new NbtByte((byte)((bool)propertyInfo.GetValue(target) ? 1 : 0));
					else
						tag[nbtTag.Name] = new NbtByte((byte)propertyInfo.GetValue(target));
					break;
				case NbtTagType.Short:
					tag[nbtTag.Name] = new NbtShort((short)propertyInfo.GetValue(target));
					break;
				case NbtTagType.Int:
					if (propertyInfo.PropertyType == typeof(bool))
						tag[nbtTag.Name] = new NbtInt((bool)propertyInfo.GetValue(target) ? 1 : 0);
					else
						tag[nbtTag.Name] = new NbtInt((int)propertyInfo.GetValue(target));
					break;
				case NbtTagType.Long:
					tag[nbtTag.Name] = new NbtLong((long)propertyInfo.GetValue(target));
					break;
				case NbtTagType.Float:
					tag[nbtTag.Name] = new NbtFloat((float)propertyInfo.GetValue(target));
					break;
				case NbtTagType.Double:
					tag[nbtTag.Name] = new NbtDouble((double)propertyInfo.GetValue(target));
					break;
				case NbtTagType.ByteArray:
					tag[nbtTag.Name] = new NbtByteArray((byte[])propertyInfo.GetValue(target));
					break;
				case NbtTagType.String:
					tag[nbtTag.Name] = new NbtString((string)propertyInfo.GetValue(target));
					break;
				case NbtTagType.List:
					break;
				case NbtTagType.Compound:
					break;
				case NbtTagType.IntArray:
					tag[nbtTag.Name] = new NbtIntArray((int[])propertyInfo.GetValue(target));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return (T)propertyInfo.GetValue(target);
		}


		private static string LowercaseFirst(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToLower(s[0]) + s.Substring(1);
		}

		public void LoadFromNbt(NbtTag dataTag)
		{
			GetPropertyValue(dataTag, () => Version);
			GetPropertyValue(dataTag, () => Initialized);
			GetPropertyValue(dataTag, () => LevelName);
			GetPropertyValue(dataTag, () => GeneratorName);
			GetPropertyValue(dataTag, () => GeneratorVersion);
			GetPropertyValue(dataTag, () => GeneratorOptions);
			GetPropertyValue(dataTag, () => RandomSeed);
			GetPropertyValue(dataTag, () => MapFeatures);
			GetPropertyValue(dataTag, () => LastPlayed);
			GetPropertyValue(dataTag, () => AllowCommands);
			GetPropertyValue(dataTag, () => Hardcore);
			GetPropertyValue(dataTag, () => GameType);
			GetPropertyValue(dataTag, () => Time);
			GetPropertyValue(dataTag, () => DayTime);
			GetPropertyValue(dataTag, () => SpawnX);
			GetPropertyValue(dataTag, () => SpawnY);
			GetPropertyValue(dataTag, () => SpawnZ);
			GetPropertyValue(dataTag, () => Raining);
			GetPropertyValue(dataTag, () => RainTime);
			GetPropertyValue(dataTag, () => Thundering);
			GetPropertyValue(dataTag, () => ThunderTime);
		}

		public void SaveToNbt(NbtTag dataTag)
		{
			SetPropertyValue(dataTag, () => Version);
			SetPropertyValue(dataTag, () => Initialized);
			SetPropertyValue(dataTag, () => LevelName);
			SetPropertyValue(dataTag, () => GeneratorName);
			SetPropertyValue(dataTag, () => GeneratorVersion);
			SetPropertyValue(dataTag, () => GeneratorOptions);
			SetPropertyValue(dataTag, () => RandomSeed);
			SetPropertyValue(dataTag, () => MapFeatures);
			SetPropertyValue(dataTag, () => LastPlayed);
			SetPropertyValue(dataTag, () => AllowCommands);
			SetPropertyValue(dataTag, () => Hardcore);
			SetPropertyValue(dataTag, () => GameType);
			SetPropertyValue(dataTag, () => Time);
			SetPropertyValue(dataTag, () => DayTime);
			SetPropertyValue(dataTag, () => SpawnX);
			SetPropertyValue(dataTag, () => SpawnY);
			SetPropertyValue(dataTag, () => SpawnZ);
			SetPropertyValue(dataTag, () => Raining);
			SetPropertyValue(dataTag, () => RainTime);
			SetPropertyValue(dataTag, () => Thundering);
			SetPropertyValue(dataTag, () => ThunderTime);
		}
	}
}