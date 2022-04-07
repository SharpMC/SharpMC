using System.IO;
using System.IO.Compression;

namespace SharpMC.Core.Utils
{
	/// <summary>
	///     DeflateStream wrapper that calculates Adler32 checksum of the written data,
	///     to allow writing ZLib header (RFC-1950).
	/// </summary>
	internal sealed class ZLibStream : DeflateStream
	{
		private const int ChecksumModulus = 65521;
		private readonly MemoryStream _buffer = new MemoryStream();
		private int _adler32A = 1, _adler32B;

		public ZLibStream(Stream stream, CompressionLevel level, bool leaveOpen)
			: base(stream, level, leaveOpen)
		{
		}

		public int Checksum
		{
			get
			{
				UpdateChecksum(_buffer.ToArray(), 0, _buffer.Length);
				return _adler32B*65536 + _adler32A;
			}
		}

		private void UpdateChecksum(byte[] data, int offset, long length)
		{
			for (long counter = 0; counter < length; ++counter)
			{
				_adler32A = (_adler32A + data[offset + counter])%ChecksumModulus;
				_adler32B = (_adler32B + _adler32A)%ChecksumModulus;
			}
		}

		public override void Write(byte[] array, int offset, int count)
		{
			_buffer.Write(array, offset, count);
			base.Write(array, offset, count);
		}
	}
}