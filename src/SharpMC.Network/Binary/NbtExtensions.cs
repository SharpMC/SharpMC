using SharpNBT;

namespace SharpMC.Network.Binary
{
    public static class NbtExtensions
    {
        public static LongArrayTag? AsLongArray(this CompoundTag tag, string name)
            => tag[name] is LongArrayTag t ? t : default;
    }
}