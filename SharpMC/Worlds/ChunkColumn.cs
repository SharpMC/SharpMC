// Distrubuted under the MIT license
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
using System.IO;
using System.Net;
using SharpMC.Blocks;
using SharpMC.Utils;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds
{
	public class ChunkColumn
	{
		private byte[] _cache;
		public BiomeBase Biome;
		public int[] BiomeColor = ArrayOf<int>.Create(256, 1);
		public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);
		public NibbleArray Blocklight = new NibbleArray(16*16*256);
		//public ushort[] Blocks = new ushort[16*16*256];
		public Block[] Blocks = new Block[16*16*256];
		public bool IsDirty = false;
		public NibbleArray Skylight = new NibbleArray(16*16*256);

		public ChunkColumn()
		{
			for (var i = 0; i < Skylight.Length; i ++)
				Skylight[i] = 0xff;
			for (var i = 0; i < BiomeColor.Length; i++)
				BiomeColor[i] = 8761930;
		}

		public int X { get; set; }
		public int Z { get; set; }

		public ushort GetBlock(int x, int y, int z)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
			{
				if (Blocks[index] != null) return (Blocks[index].Id);
				return 0;
			}
			return 900;
		}

		public Block GetABlock(int x, int y, int z)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
			{
				if (Blocks[index] != null) return (Blocks[index]);
				return new BlockAir();
			}
			return new Block(0);
		}

		public byte GetMetadata(int x, int y, int z)
		{
			return (byte) (GetABlock(x, y, z).Metadata);
		}

		public void SetBlock(int x, int y, int z, Block block)
		{
			var index = x + 16*z + 16*16*y;
			if (index >= 0 && index < Blocks.Length)
				//Blocks[index] = Convert.ToUInt16((block.Id << 4) | block.Metadata);
				Blocks[index] = block;
		}

		public void SetBlocklight(int x, int y, int z, byte data)
		{
			_cache = null;
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
			_cache = null;
			Skylight[(x*2048) + (z*256) + y] = data;
		}

		public byte[] GetMeta()
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new NbtBinaryWriter(stream, true))
				{
					writer.Write(IPAddress.HostToNetworkOrder(X));
					writer.Write(IPAddress.HostToNetworkOrder(Z));
					writer.Write(true);
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

					foreach (var i in Blocks)
						writer.Write((ushort) ((i.Id << 4) | i.Metadata));

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
			var writer = new MSGBuffer(new byte[0]);
			if (!unloader)
			{
				writer.WriteInt(X);
				writer.WriteInt(Z);
				writer.WriteBool(true);
				writer.WriteUShort(0xffff); // bitmap
				writer.WriteVarInt((Blocks.Length*2) + Skylight.Data.Length + Blocklight.Data.Length + BiomeId.Length);

				foreach (var i in Blocks)
				{
					if (i == null) writer.WriteUShort((0 << 4) | 0);
					else writer.WriteUShort((ushort) ((i.Id << 4) | i.Metadata));
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
			var buffer = new MSGBuffer(new byte[0]);

			buffer.WriteInt(Blocks.Length);
			foreach (var i in Blocks)
				buffer.WriteUShort(i != null ? (ushort) i.Id : (ushort) 0);

			buffer.WriteInt(Blocks.Length);
			foreach (var i in Blocks)
				buffer.WriteUShort(i != null ? i.Metadata : (ushort) 0);

			buffer.WriteInt(Blocklight.Data.Length);
			buffer.Write(Blocklight.Data);

			buffer.WriteInt(Skylight.Data.Length);
			buffer.Write(Skylight.Data);

			buffer.WriteInt(BiomeId.Length);
			buffer.Write(BiomeId);

			return buffer.ExportWriter;
		}
	}
}