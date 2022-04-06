// Distributed under the MIT license
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
using System.IO;
using fNbt;
using SharpMC.Core.Blocks;
using SharpMC.Core.TileEntities;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds.Standard;

namespace SharpMC.Core.Worlds.Anvil
{
	public class AnvilWorldProvider : WorldProvider
	{
		private readonly StandardWorldProvider _backEndGenerator;

		private readonly Dictionary<Tuple<int,int>, ChunkColumn> _chunkCache =
			new Dictionary<Tuple<int, int>, ChunkColumn>();

		private string _basePath;
		private LevelInfo _level;
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
			if (File.Exists(Path.Combine(_basePath, "level.dat")))
			{
				file.LoadFromFile(Path.Combine(_basePath, "level.dat"));
				var dataTag = file.RootTag["Data"];
				_level = new LevelInfo(dataTag);
			}
			else
			{
				throw new Exception(@"Could not load Anvil world!");
			}

			_waterOffsetY = (byte)Config.GetProperty("PCWaterOffset", 0);
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
				var coords = new Tuple<int,int>(chunkCoordinates.X, chunkCoordinates.Z);

				if (_chunkCache.ContainsKey(coords))
				{
					return _chunkCache[coords];
				}

				var chunk = GetChunk(chunkCoordinates.X, chunkCoordinates.Z);
				if (!_chunkCache.ContainsKey(coords))
				{
					_chunkCache.Add(coords, chunk);
				}

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

				//if (offset == 0 || length == 0) return _backEndGenerator.GenerateChunkColumn(new Vector2(X, Z));

				if (offset == 0 || length == 0)
				{
					return _backEndGenerator.GenerateChunkColumn(new Vector2(X,Z));
				}

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
				var tileEntities = dataTag["TileEntities"] as NbtList;

				if (tileEntities != null)
				{
					foreach (var nbtTag in tileEntities)
					{
						var blockEntityTag = (NbtCompound)nbtTag;
						string entityId = blockEntityTag["id"].StringValue;
						int x = blockEntityTag["x"].IntValue;
						int y = blockEntityTag["y"].IntValue - _waterOffsetY;
						int z = blockEntityTag["z"].IntValue;
						blockEntityTag["y"] = new NbtInt("y", y);

						TileEntity blockEntity = TileEntityFactory.GetBlockEntityById(entityId);
						if (blockEntity != null)
						{
							blockEntityTag.Name = string.Empty;
							chunk.SetBlockEntity(new Vector3(x, y, z), blockEntityTag);
						}
					}
				}

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

		public override ChunkColumn LoadChunk(int x, int y)
		{
			return GetChunk(x, y);
		}

		/*public override void SetBlock(Block block, Level level, bool broadcast)
		{
			/*==ChunkColumn c;
			lock (_chunkCache)
			{
				if (
					!_chunkCache.TryGetValue(new ChunkCoordinates((int)block.Coordinates.X >> 4, (int)block.Coordinates.Z >> 4), out c))
					throw new Exception("No chunk found!");
			}

			c.SetBlock(((int)block.Coordinates.X & 0x0f), ((int)block.Coordinates.Y & 0x7f), ((int)block.Coordinates.Z & 0x0f),
				block);
			if (!broadcast) return;

			BlockChange.Broadcast(block, level);
		}*/
	}
}