using System;
using System.Collections;
using Level = SharpMC.World.Level;

namespace SharpMC.Blocks
{
	public class BlockRedstoneDust : RedstoneDevice
	{
		internal BlockRedstoneDust() : base(55)
		{
		}

		public override void RedstoneTick(Level level)
		{

		}

		public void SetPowerLevel(int target, Level lvl)
		{
			if (target < 0 || target > 15) throw new IndexOutOfRangeException("target");

			var newbits = new BitArray(new int[] { target });
			var rawbits = new BitArray(new byte[] { Metadata });

			rawbits[0] = newbits[0];
			rawbits[1] = newbits[1];
			rawbits[2] = newbits[2];
			rawbits[3] = newbits[3];

			Metadata = ConvertToByte(rawbits);
			lvl.SetBlock(this);
		}

		public int GetPowerLevel()
		{
			var newbits = new BitArray(new byte[] { 0x00 });
			var rawbits = new BitArray(new byte[] { Metadata });

			newbits[0] = rawbits[0];
			newbits[1] = rawbits[1];
			newbits[2] = rawbits[2];
			newbits[3] = rawbits[3];

			return ConvertToByte(newbits);
		}
	}
}