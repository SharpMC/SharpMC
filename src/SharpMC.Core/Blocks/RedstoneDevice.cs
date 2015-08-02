using System;
using System.Collections;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Blocks
{
	public class RedstoneDevice : Block
	{
		internal RedstoneDevice(ushort id) : base(id)
		{
		}

		public virtual void RedstoneTick(Level world)
		{
			
		}

		public override void DoPhysics(Level level)
		{
			
		}

		public byte ConvertToByte(BitArray bits)
		{
			if (bits.Count != 8)
			{
				throw new ArgumentException("bits");
			}
			byte[] bytes = new byte[1];
			bits.CopyTo(bytes, 0);
			return bytes[0];
		}
	}
}
