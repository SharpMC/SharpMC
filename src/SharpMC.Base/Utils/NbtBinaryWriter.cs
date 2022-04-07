using System;
using System.IO;
using System.Net;
using System.Text;
using fNbt;

namespace SharpMC.Core.Utils
{
	/// <summary>
	///     BinaryWriter wrapper that takes care of writing primitives to an NBT stream,
	///     while taking care of endianness and string encoding.
	/// </summary>
	internal class NbtBinaryWriter : BinaryWriter
	{
		private readonly bool _bigEndian;

		public NbtBinaryWriter(Stream input, bool bigEndian)
			: base(input)
		{
			this._bigEndian = bigEndian;
		}

		public void Write(NbtTagType value)
		{
			Write((byte) value);
		}

		public void WriteVarInt(int integer)
		{
			while ((integer & -128) != 0)
			{
				Write((byte) (integer & 127 | 128));
				integer = (int) ((uint) integer >> 7);
			}
			Write((byte) integer);
		}

		public override void Write(short value)
		{
			if (BitConverter.IsLittleEndian == _bigEndian)
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
			if (BitConverter.IsLittleEndian == _bigEndian)
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
			if (BitConverter.IsLittleEndian == _bigEndian)
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
			if (BitConverter.IsLittleEndian == _bigEndian)
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