using System.Linq;
using SharpNBT;

namespace SharpMC.Network.Binary.Model
{
    public class HeightMaps : INbtSerializable
    {
        public long[] MotionBlocking { get; set; }
        public long[] WorldSurface { get; set; }

        public CompoundTag ToCompound()
        {
            var tag = new CompoundTag(null)
            {
                new LongArrayTag("MOTION_BLOCKING", MotionBlocking),
                new LongArrayTag("WORLD_SURFACE", WorldSurface)
            };
            return tag;
        }

        public void ToObject(CompoundTag tag)
        {
            var motBlock = (LongArrayTag) tag["MOTION_BLOCKING"];
            var worSurface = (LongArrayTag) tag["WORLD_SURFACE"];
            MotionBlocking = motBlock.ToArray();
            WorldSurface = worSurface.ToArray();
        }
    }
}