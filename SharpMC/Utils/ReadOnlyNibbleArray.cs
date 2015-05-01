using System;
using System.Collections.ObjectModel;

namespace SharpMC.Utils
{
	public class ReadOnlyNibbleArray
	{
		public ReadOnlyNibbleArray(NibbleArray array)
		{
			NibbleArray = array;
		}

		private NibbleArray NibbleArray { get; set; }

		public byte this[int index]
		{
			get { return NibbleArray[index]; }
		}

		public ReadOnlyCollection<byte> Data
		{
			get { return Array.AsReadOnly(NibbleArray.Data); }
		}
	}
}