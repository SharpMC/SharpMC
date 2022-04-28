using System;
using System.Collections;
using System.Numerics;
using SharpMC.API.Worlds;
using SharpMC.Items;
using SharpMC.World;
using static SharpMC.Items.Usables.UsableItem;

namespace SharpMC.Blocks.Logic
{
    public class RedstoneDustBlock : RedstoneDevice
    {
        internal RedstoneDustBlock() : base(55)
        {
        }

        public override void RedstoneTick(Level level)
        {
        }

        public void SetPowerLevel(int target, IItemLevel lvl)
        {
            if (target < 0 || target > 15)
                throw new IndexOutOfRangeException(nameof(target));

            var newbits = new BitArray(new int[] {target});
            var rawbits = new BitArray(new byte[] {42});

            rawbits[0] = newbits[0];
            rawbits[1] = newbits[1];
            rawbits[2] = newbits[2];
            rawbits[3] = newbits[3];

            // TODO
            lvl.SetBlock(this, Vector3.One);
        }

        public int GetPowerLevel()
        {
            var newbits = new BitArray(new byte[] {0x00});
            var rawbits = new BitArray(new byte[] {42});

            newbits[0] = rawbits[0];
            newbits[1] = rawbits[1];
            newbits[2] = rawbits[2];
            newbits[3] = rawbits[3];

            return ConvertToByte(newbits);
        }
    }
}