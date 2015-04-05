using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using fNbt;

namespace SharpMCRewrite.Utils
{
	/// <summary>
	///     BinaryReader wrapper that takes care of reading primitives from an NBT stream,
	///     while taking care of endianness, string encoding, and skipping.
	/// </summary>
	public class NbtBinaryReader : BinaryReader
	{
		private const int SeekBufferSize = 64*1024;
		private readonly bool bigEndian;

		private readonly byte[] floatBuffer = new byte[sizeof (float)],
			doubleBuffer = new byte[sizeof (double)];

		private byte[] seekBuffer;

		public NbtBinaryReader(Stream input, bool bigEndian)
			: base(input)
		{
			this.bigEndian = bigEndian;
		}

		public TagSelector Selector { get; set; }

		public NbtTagType ReadTagType()
		{
			var type = (NbtTagType) ReadByte();
			if (type < NbtTagType.End || type > NbtTagType.IntArray)
			{
				throw new NbtFormatException("NBT tag type out of range: " + (int) type);
			}
			return type;
		}

		public override short ReadInt16()
		{
			return BitConverter.IsLittleEndian == bigEndian ? NbtBinaryWriter.Swap(base.ReadInt16()) : base.ReadInt16();
		}

		public override int ReadInt32()
		{
			return BitConverter.IsLittleEndian == bigEndian ? NbtBinaryWriter.Swap(base.ReadInt32()) : base.ReadInt32();
		}

		public override long ReadInt64()
		{
			return BitConverter.IsLittleEndian == bigEndian ? NbtBinaryWriter.Swap(base.ReadInt64()) : base.ReadInt64();
		}

		public override float ReadSingle()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				BaseStream.Read(floatBuffer, 0, sizeof (float));
				Array.Reverse(floatBuffer);
				return BitConverter.ToSingle(floatBuffer, 0);
			}
			return base.ReadSingle();
		}

		public override double ReadDouble()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				BaseStream.Read(doubleBuffer, 0, sizeof (double));
				Array.Reverse(doubleBuffer);
				return BitConverter.ToDouble(doubleBuffer, 0);
			}
			return base.ReadDouble();
		}

		public override string ReadString()
		{
			var length = ReadInt16();
			if (length < 0)
			{
				throw new NbtFormatException("Negative string length given!");
			}
			var stringData = ReadBytes(length);
			return Encoding.UTF8.GetString(stringData);
		}

		public void Skip(int bytesToSkip)
		{
			if (bytesToSkip < 0)
			{
				throw new ArgumentOutOfRangeException("bytesToSkip");
			}
			if (BaseStream.CanSeek)
			{
				BaseStream.Position += bytesToSkip;
			}
			else if (bytesToSkip != 0)
			{
				if (seekBuffer == null)
					seekBuffer = new byte[SeekBufferSize];
				var bytesDone = 0;
				while (bytesDone < bytesToSkip)
				{
					var readThisTime = BaseStream.Read(seekBuffer, bytesDone, bytesToSkip - bytesDone);
					if (readThisTime == 0)
					{
						throw new EndOfStreamException();
					}
					bytesDone += readThisTime;
				}
			}
		}

		public void SkipString()
		{
			var length = ReadInt16();
			if (length < 0)
			{
				throw new NbtFormatException("Negative string length given!");
			}
			Skip(length);
		}
	}

	/// <summary>
	///     DeflateStream wrapper that calculates Adler32 checksum of the written data,
	///     to allow writing ZLib header (RFC-1950).
	/// </summary>
	internal sealed class ZLibStream : DeflateStream
	{
		private const int ChecksumModulus = 65521;
		private readonly MemoryStream _buffer = new MemoryStream();
		private int adler32A = 1, adler32B;

		public ZLibStream(Stream stream, CompressionLevel level, bool leaveOpen)
			: base(stream, level, leaveOpen)
		{
		}

		public int Checksum
		{
			get
			{
				UpdateChecksum(_buffer.ToArray(), 0, _buffer.Length);
				return ((adler32B*65536) + adler32A);
			}
		}

		private void UpdateChecksum(byte[] data, int offset, long length)
		{
			for (long counter = 0; counter < length; ++counter)
			{
				adler32A = (adler32A + (data[offset + counter]))%ChecksumModulus;
				adler32B = (adler32B + adler32A)%ChecksumModulus;
			}
		}

		public override void Write(byte[] array, int offset, int count)
		{
//			UpdateChecksum(array, offset, count);
			_buffer.Write(array, offset, count);
			base.Write(array, offset, count);
		}
	}
}