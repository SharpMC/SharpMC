#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Worlds
{
	using System.IO;
	using System.Net;

	using SharpMC.Blocks;
	using SharpMC.Utils;
	using SharpMC.Worlds.Standard.BiomeSystem;

	public class ChunkColumn
	{
		private byte[] _cache;

		public BiomeBase Biome;

		public int[] BiomeColor = ArrayOf<int>.Create(256, 1);

		public byte[] BiomeId = ArrayOf<byte>.Create(256, 1);

		public NibbleArray Blocklight = new NibbleArray(16 * 16 * 256);

		public ushort[] Blocks = new ushort[16 * 16 * 256];

		public bool IsDirty = false;

		public short[] Metadata = new short[16 * 16 * 256];

		public NibbleArray Skylight = new NibbleArray(16 * 16 * 256);

		public ChunkColumn()
		{
			for (var i = 0; i < this.Skylight.Length; i ++)
			{
				this.Skylight[i] = 0xff;
			}

			for (var i = 0; i < this.BiomeColor.Length; i++)
			{
				this.BiomeColor[i] = 8761930;
			}

			for (var i = 0; i < this.Metadata.Length; i++)
			{
				this.Metadata[i] = 0;
			}
		}

		public Level Level { get; set; }

		public int X { get; set; }

		public int Z { get; set; }

		public ushort GetBlock(int x, int y, int z)
		{
			var index = x + 16 * z + 16 * 16 * y;
			if (index >= 0 && index < this.Blocks.Length)
			{
				return this.Blocks[index];
			}

			return 0x0;
		}

		public byte GetMetadata(int x, int y, int z)
		{
			var index = x + 16 * z + 16 * 16 * y;
			if (index >= 0 && index < this.Metadata.Length)
			{
				return (byte)this.Metadata[index];
			}

			return 0x0;
		}

		public void SetMetadata(int x, int y, int z, byte metadata)
		{
			var index = x + 16 * z + 16 * 16 * y;
			if (index >= 0 && index < this.Metadata.Length)
			{
				this.Metadata[index] = metadata;
			}
		}

		public void SetBlock(int x, int y, int z, Block block)
		{
			var index = x + 16 * z + 16 * 16 * y;
			if (index >= 0 && index < this.Blocks.Length)
			{
				this.Blocks[index] = block.Id;
				this.Metadata[index] = block.Metadata;
			}
		}

		public void SetBlocklight(int x, int y, int z, byte data)
		{
			this._cache = null;
			this.Blocklight[(x * 2048) + (z * 256) + y] = data;
		}

		public byte GetBlocklight(int x, int y, int z)
		{
			return this.Blocklight[(x * 2048) + (z * 256) + y];
		}

		public byte GetSkylight(int x, int y, int z)
		{
			return this.Skylight[(x * 2048) + (z * 256) + y];
		}

		public void SetSkylight(int x, int y, int z, byte data)
		{
			this._cache = null;
			this.Skylight[(x * 2048) + (z * 256) + y] = data;
		}

		public byte[] GetMeta()
		{
			using (var stream = new MemoryStream())
			{
				using (var writer = new NbtBinaryWriter(stream, true))
				{
					writer.Write(IPAddress.HostToNetworkOrder(this.X));
					writer.Write(IPAddress.HostToNetworkOrder(this.Z));

					// 	writer.Write(true);
					writer.Write((ushort)0xffff); // bitmap

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
					writer.WriteVarInt(
						(this.Blocks.Length * 2) + this.Skylight.Data.Length + this.Blocklight.Data.Length + this.BiomeId.Length);

					for (var i = 0; i < this.Blocks.Length; i++)
					{
						writer.Write((ushort)((this.Blocks[i] << 4) | this.Metadata[i]));
					}

					writer.Write(this.Blocklight.Data);
					writer.Write(this.Skylight.Data);

					writer.Write(this.BiomeId);

					writer.Flush();
					writer.Close();
				}

				return stream.ToArray();
			}
		}

		public byte[] GetBytes(bool unloader = false)
		{
			var writer = new LocalDataBuffer(new byte[0]);
			if (!unloader)
			{
				writer.WriteInt(this.X);
				writer.WriteInt(this.Z);
				writer.WriteBool(true);
				writer.WriteUShort(0xffff); // bitmap
				writer.WriteVarInt(
					(this.Blocks.Length * 2) + this.Skylight.Data.Length + this.Blocklight.Data.Length + this.BiomeId.Length);

				for (var i = 0; i < this.Blocks.Length; i++)
				{
					// if (i == null) writer.WriteUShort((0 << 4) | 0);
					writer.WriteUShort((ushort)((this.Blocks[i] << 4) | this.Metadata[i]));
				}

				writer.Write(this.Blocklight.Data);
				writer.Write(this.Skylight.Data);

				writer.Write(this.BiomeId);
			}
			else
			{
				writer.WriteInt(this.X);
				writer.WriteInt(this.Z);
				writer.WriteBool(true);
				writer.WriteUShort(0);
				writer.WriteVarInt(0);
			}

			return writer.ExportWriter;
		}

		public byte[] Export()
		{
			var buffer = new LocalDataBuffer(new byte[0]);

			buffer.WriteInt(this.Blocks.Length);

			for (var i = 0; i < this.Blocks.Length; i++)
			{
				buffer.WriteUShort(this.Blocks[i]);
			}

			buffer.WriteInt(this.Blocks.Length);
			for (var i = 0; i < this.Blocks.Length; i++)
			{
				buffer.WriteUShort((ushort)this.Metadata[i]);
			}

			buffer.WriteInt(this.Blocklight.Data.Length);
			buffer.Write(this.Blocklight.Data);

			buffer.WriteInt(this.Skylight.Data.Length);
			buffer.Write(this.Skylight.Data);

			buffer.WriteInt(this.BiomeId.Length);
			buffer.Write(this.BiomeId);

			return buffer.ExportWriter;
		}
	}
}