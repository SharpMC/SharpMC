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

using System;
using System.IO;
using System.Net;
using System.Text;
using fNbt;

namespace SharpMC.Utils
{
	/// <summary>
	///     BinaryWriter wrapper that takes care of writing primitives to an NBT stream,
	///     while taking care of endianness and string encoding.
	/// </summary>
	internal class NbtBinaryWriter : BinaryWriter
	{
		private readonly bool bigEndian;

		public NbtBinaryWriter(Stream input, bool bigEndian)
			: base(input)
		{
			this.bigEndian = bigEndian;
		}

		public void Write(NbtTagType value)
		{
			Write((byte) value);
		}

		public void WriteVarInt(int Integer)
		{
			while ((Integer & -128) != 0)
			{
				Write((byte) (Integer & 127 | 128));
				Integer = (int) (((uint) Integer) >> 7);
			}
			Write((byte) Integer);
		}

		public override void Write(short value)
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				base.Write(IPAddress.HostToNetworkOrder(value));
			}
			else
			{
				base.Write(value);
			}
		}

		/*public override void Write(int value)
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				base.Write(Swap(value));
			}
			else
			{
				base.Write(value);
			}
		}*/


		public override void Write(long value)
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				base.Write(Swap(value));
			}
			else
			{
				base.Write(value);
			}
		}

		public override void Write(float value)
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				var floatBytes = BitConverter.GetBytes(value);
				Array.Reverse(floatBytes);
				Write(floatBytes);
			}
			else
			{
				base.Write(value);
			}
		}

		public override void Write(double value)
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				var doubleBytes = BitConverter.GetBytes(value);
				Array.Reverse(doubleBytes);
				Write(doubleBytes);
			}
			else
			{
				base.Write(value);
			}
		}

		public override void Write(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			var bytes = Encoding.UTF8.GetBytes(value);
			Write((short) bytes.Length);
			Write(bytes);
		}

		public static short Swap(short v)
		{
			return (short) ((v >> 8) & 0x00FF |
			                (v << 8) & 0xFF00);
		}

		public static int Swap(int v)
		{
			var v2 = (uint) v;
			return (int) ((v2 >> 24) & 0x000000FF |
			              (v2 >> 8) & 0x0000FF00 |
			              (v2 << 8) & 0x00FF0000 |
			              (v2 << 24) & 0xFF000000);
		}

		public static long Swap(long v)
		{
			return (Swap((int) v) & uint.MaxValue) << 32 |
			       Swap((int) (v >> 32)) & uint.MaxValue;
		}
	}
}