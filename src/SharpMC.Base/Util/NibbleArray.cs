using System;

namespace SharpMC.Util
{
	//Credits to https://github.com/NiclasOlofsson/MiNET
	public class NibbleArray : ICloneable
	{
		public byte[] Data { get; set; }

		public NibbleArray()
		{
		}

		public NibbleArray(int length)
		{
			Data = new byte[length / 2];
		}

		public int Length
		{
			get { return Data.Length * 2; }
		}

		public byte this[int index]
		{
			get { return (byte)(Data[index / 2] >> (index % 2 * 4) & 0xF); }
			set
			{
				value &= 0xF;
				Data[index / 2] &= (byte)(0xF << ((index + 1) % 2 * 4));
				Data[index / 2] |= (byte)(value << (index % 2 * 4));
			}
		}

		public object Clone()
		{
			var clone = new NibbleArray
			{
				Data = (byte[]) Data.Clone()
			};
			return clone;
		}
	}
}
