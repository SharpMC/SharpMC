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