using System;
using System.IO;
using System.Net;
using Craft.Net.Anvil;
using MiNET.Utils;
using SharpMCRewrite.Blocks;

namespace SharpMCRewrite.Worlds
{
	public class ChunkColumn
	{
		private byte[] _cache;
		public int[] BiomeColor = ArrayOf<int>.Create(256, 1);
		public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);
		public NibbleArray Blocklight = new NibbleArray(16*16*256);
		public ushort[] Blocks = new ushort[16*16*256];
		public NibbleArray Skylight = new NibbleArray(16*16*256);

		public ChunkColumn()
		{
			for (var i = 0; i < Skylight.Length; i ++)
				Skylight[i] = 0xff;
			for (var i = 0; i < BiomeColor.Length; i++)
				BiomeColor[i] = 8761930; // Grass color?
		}

		public int X { get; set; }
		public int Z { get; set; }

		public ushort GetBlock(int x, int y, int z)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
				return (ushort) (Blocks[index] >> 4);
			return 900;
		}

		public byte GetMetadata(int x, int y, int z)
		{
			//return metadata[(x * 2048) + (z * 128) + y];
			return 0; //We dont support METADATA for now :P
		}

		public void SetBlock(int x, int y, int z, Block block)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
				Blocks[index] = Convert.ToUInt16((block.Id << 4) | block.Metadata);
		}

		public void SetBlocklight(int x, int y, int z, byte data)
		{
			_cache = null;
			Blocklight[(x*2048) + (z*256) + y] = data;
		}

		public void SetSkylight(int x, int y, int z, byte data)
		{
			_cache = null;
			Skylight[(x*2048) + (z*256) + y] = data;
		}

		public byte[] GetBytes()
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new NbtBinaryWriter(stream, true))
				{
					writer.Write(IPAddress.HostToNetworkOrder(X));
					writer.Write(IPAddress.HostToNetworkOrder(Z));
					writer.Write(true);
					writer.Write((ushort) 0xffff); // bitmap
					writer.WriteVarInt((Blocks.Length*2) + Skylight.Data.Length + Blocklight.Data.Length + BiomeId.Length);

					foreach (var i in Blocks)
						writer.Write(i);

					writer.Write(Blocklight.Data);
					writer.Write(Skylight.Data);

					writer.Write(BiomeId); //OK

					writer.Flush();
					writer.Close();
				}
				return stream.ToArray();
			}
		}

		public byte[] Export()
		{
			var buffer = new MSGBuffer(new byte[0]);

			buffer.WriteInt(Blocks.Length);
			foreach (var i in Blocks)
				buffer.WriteUShort(i);

			buffer.WriteInt(Blocklight.Data.Length);
			buffer.Write(Blocklight.Data);

			buffer.WriteInt(Skylight.Data.Length);
			buffer.Write(Skylight.Data);

			buffer.WriteInt(BiomeId.Length);
			buffer.Write(BiomeId);

			return buffer.ExportWriter;
		}
	}

	public static class ArrayOf<T> where T : new()
	{
		public static T[] Create(int size, T initialValue)
		{
			var array = new T[size];
			for (var i = 0; i < array.Length; i++)
				array[i] = initialValue;
			return array;
		}

		public static T[] Create(int size)
		{
			var array = new T[size];
			for (var i = 0; i < array.Length; i++)
				array[i] = new T();
			return array;
		}
	}
}