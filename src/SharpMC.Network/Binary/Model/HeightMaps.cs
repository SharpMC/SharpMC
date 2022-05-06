using System.Linq;
using SharpNBT;

namespace SharpMC.Network.Binary.Model
{
    public class HeightMaps : INbtSerializable
    {
        public long[]? MotionBlocking { get; set; }
        public long[]? WorldSurface { get; set; }

        public CompoundTag ToCompound()
        {
            var tag = new CompoundTag(null);
            if (MotionBlocking != null)
                tag.Add(new LongArrayTag("MOTION_BLOCKING", MotionBlocking));
            if (WorldSurface != null)
                tag.Add(new LongArrayTag("WORLD_SURFACE", WorldSurface));
            return tag;
        }

        public void ToObject(CompoundTag tag)
        {
            var motBlock = tag.AsLongArray("MOTION_BLOCKING");
            var worSurface = tag.AsLongArray("WORLD_SURFACE");
            MotionBlocking = motBlock?.ToArray();
            WorldSurface = worSurface?.ToArray();
        }
    }
}