using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using fNbt;
using SharpMC.Blocks;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds.Standard.BiomeSystem;
using SharpMC.Network.Util;
using ChunkCoordinates = SharpMC.Util.ChunkCoordinates;
using NibbleArray = SharpMC.Util.NibbleArray;

namespace SharpMC.World
{
	public class ChunkColumn : IDisposable
	{
		public const int Height = 256;
		public const int Width = 16;
		public const int Depth = 16;

		public ChunkCoordinates Coordinates { get; }
		private ChunkSection[] Sections { get; }
		private byte[] Biomes { get; }

		public bool IsDirty { get; internal set; } = false;
        
		private byte[] Cache { get; set; } = null;

		public byte[] HeightMap = new byte[Width * Depth];

		public ChunkColumn(ChunkCoordinates coordinates)
		{
			Coordinates = coordinates;
			Sections = new ChunkSection[16];
			Biomes = new byte[Depth * Width];

			for (int i = 0; i < Sections.Length; i++)
			{
				Sections[i] = new ChunkSection();
			}

			for (int i = 0; i < Biomes.Length; i++)
			{
				Biomes[i] = 1; //Plains
			}

			for (int i = 0; i < HeightMap.Length; i++)
			{
				HeightMap[i] = 0;
			}
		}

		private ChunkSection GetChunkSection(int y)
		{
			return Sections[y >> 4];
		}

		public short GetBlockId(int x, int y, int z)
		{
			return GetChunkSection(y).GetBlockId(x, y - 16 * (y >> 4), z);
		}

		public void SetBlockId(int x, int y, int z, short id)
		{
			GetChunkSection(y).SetBlockId(x, y - 16*(y >> 4), z, id);

			Cache = null;
			IsDirty = true;
		}

		public byte GetBlockData(int x, int y, int z)
		{
			return GetChunkSection(y).GetBlockData(x, y - 16 * (y >> 4), z);
		}

		public void SetBlockData(int x, int y, int z, byte meta)
		{
			GetChunkSection(y).SetBlockData(x, y - 16 * (y >> 4), z, meta);

			Cache = null;
			IsDirty = true;
		}

		public void SetBiome(int x, int z, byte biome)
		{
			Biomes[(z << 4) + (x)] = biome;

			Cache = null;
			IsDirty = true;
		}

		public byte GetBiome(int x, int z)
		{
			return Biomes[(z << 4) + (x)];
		}

		public void RecalcHeight()
		{
			for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++)
				{
					for (byte y = 127; y > 0; y--)
					{
						if (GetBlockId(x, y, z) != 0)
						{
							HeightMap[(x << 4) + z] = (byte)(y + 1);
							//..SetHeight(x, z, (byte)(y + 1));
							break;
						}
					}
				}
			}
		}

		public byte[] ToArray()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (MinecraftStream m = new MinecraftStream(ms))
				{
					WriteTo(m);
				}
				return ms.ToArray();
			}
		}

		public void WriteTo(MinecraftStream stream)
		{
			if (Cache != null)
			{
				stream.Write(Cache);
				return;
			}

			byte[] sectionData;
			int sectionBitmask = 0;
			using (MemoryStream ms = new MemoryStream())
			{
				using (MinecraftStream mc = new MinecraftStream(ms))
				{
					for (int i = 0; i < Sections.Length; i++)
					{
						ChunkSection section = Sections[i];
						if (section.IsAllAir) continue;

						sectionBitmask |= 1 << i;

						section.WriteTo(mc, true);
					}
				}
				sectionData = ms.ToArray();
			}

			using (MemoryStream ms = new MemoryStream())
			{
				using (MinecraftStream mc = new MinecraftStream(ms))
				{
					mc.WriteInt(Coordinates.X);
					mc.WriteInt(Coordinates.Z);

					mc.WriteBool(true);

					mc.WriteVarInt(sectionBitmask); //Primary Bitmask

					mc.WriteVarInt(sectionData.Length + 256);
					mc.Write(sectionData, 0, sectionData.Length);
					mc.Write(Biomes);

					mc.WriteVarInt(0); //No block entities for now
				}

				Cache = ms.ToArray();
			}

			stream.Write(Cache);
		}

		public BiomeBase Biome;
		public int[] BiomeColor = ArrayOf<int>.Create(256, 1);
		public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);
		public NibbleArray Blocklight = new NibbleArray(16*16*256);
		public ushort[] Blocks = new ushort[16*16*256];
		
		public ushort[] Metadata = new ushort[16*16*256];
		public NibbleArray Skylight = new NibbleArray(16*16*256);
		public IDictionary<Vector3, NbtCompound> TileEntities = new Dictionary<Vector3, NbtCompound>();

		public ChunkColumn()
		{
			for (var i = 0; i < Skylight.Length; i ++)
				Skylight[i] = 0xff;
			for (var i = 0; i < BiomeColor.Length; i++)
				BiomeColor[i] = 8761930;
			for (var i = 0; i < Metadata.Length; i++)
				Metadata[i] = 0;
		}

		public Level Level { get; set; }
		public int X { get; set; }
		public int Z { get; set; }

		public ushort GetBlock(int x, int y, int z)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
			{
				return (Blocks[index]);
			}
			return 0x0;
		}

		public byte GetMetadata(int x, int y, int z)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Metadata.Length)
			{
				return (byte) (Metadata[index]);
			}
			return 0x0;
		}

		public void SetMetadata(int x, int y, int z, byte metadata)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Metadata.Length)
			{
				Metadata[index] = metadata;
			}
		}

		public void SetBlock(int x, int y, int z, Block block)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
			{
				Blocks[index] = block.Id;
				Metadata[index] = block.Metadata;
			}
		}

		public void SetBlocklight(int x, int y, int z, byte data)
		{
			Blocklight[(x*2048) + (z*256) + y] = data;
		}

		public byte GetBlocklight(int x, int y, int z)
		{
			return Blocklight[(x*2048) + (z*256) + y];
		}

		public byte GetSkylight(int x, int y, int z)
		{
			return Skylight[(x*2048) + (z*256) + y];
		}

		public void SetSkylight(int x, int y, int z, byte data)
		{
			Skylight[(x*2048) + (z*256) + y] = data;
		}

		public NbtCompound GetBlockEntity(Vector3 coordinates)
		{
			NbtCompound nbt;
			TileEntities.TryGetValue(coordinates, out nbt);
			return nbt;
		}

		public void SetBlockEntity(Vector3 coordinates, NbtCompound nbt)
		{
			IsDirty = true;
			TileEntities[coordinates] = nbt;
		}

		public void RemoveBlockEntity(Vector3 coordinates)
		{
			IsDirty = true;
			TileEntities.Remove(coordinates);
		}

		public byte[] GetMeta()
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new NbtBinaryWriter(stream, true))
				{
					writer.Write(IPAddress.HostToNetworkOrder(X));
					writer.Write(IPAddress.HostToNetworkOrder(Z));
					writer.Write((ushort) 0xffff); // bitmap

					writer.Flush();
					writer.Close();
				}
				return stream.ToArray();
			}
		}

		public byte[] GetChunkData()
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new NbtBinaryWriter(stream, true))
				{
					writer.WriteVarInt((Blocks.Length*2) + Skylight.Data.Length + Blocklight.Data.Length + BiomeId.Length);

					for (var i = 0; i < Blocks.Length; i++)
						writer.Write((ushort) ((Blocks[i] << 4) | Metadata[i]));

					writer.Write(Blocklight.Data);
					writer.Write(Skylight.Data);

					writer.Write(BiomeId);

					writer.Flush();
					writer.Close();
				}
				return stream.ToArray();
			}
		}

		public byte[] GetBytes(bool unloader = false)
		{
			var writer = new DataBuffer(new byte[0]);
			if (!unloader)
			{
				writer.WriteInt(X);
				writer.WriteInt(Z);
				writer.WriteBool(true);
				writer.WriteUShort(0xffff); // bitmap
				writer.WriteVarInt((Blocks.Length*2) + Skylight.Data.Length + Blocklight.Data.Length + BiomeId.Length);

				for (var i = 0; i < Blocks.Length; i++)
				{
					writer.WriteUShort((ushort) ((Blocks[i] << 4) | Metadata[i]));
				}

				writer.Write(Blocklight.Data);
				writer.Write(Skylight.Data);

				writer.Write(BiomeId);
			}
			else
			{
				writer.WriteInt(X);
				writer.WriteInt(Z);
				writer.WriteBool(true);
				writer.WriteUShort(0);
				writer.WriteVarInt(0);
			}
			return writer.ExportWriter;
		}

		public byte[] Export()
		{
			var buffer = new DataBuffer(new byte[0]);

			buffer.WriteInt(Blocks.Length);

			for (var i = 0; i < Blocks.Length; i++)
				buffer.WriteUShort(Blocks[i]);

			buffer.WriteInt(Blocks.Length);
			for (var i = 0; i < Blocks.Length; i++)
				buffer.WriteUShort((ushort) Metadata[i]);

			buffer.WriteInt(Blocklight.Data.Length);
			buffer.Write(Blocklight.Data);

			buffer.WriteInt(Skylight.Data.Length);
			buffer.Write(Skylight.Data);

			buffer.WriteInt(BiomeId.Length);
			buffer.Write(BiomeId);

			return buffer.ExportWriter;
		}

		private bool disposed = false;
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					Blocks = null;
					Metadata = null;
					Blocklight.Data = null;
					Blocklight = null;
					BiomeId = null;
					BiomeColor = null;
					Skylight.Data = null;
					Skylight = null;
				}

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}