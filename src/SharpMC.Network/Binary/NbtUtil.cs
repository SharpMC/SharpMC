using System.IO;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public static class NbtUtil
    {
        private const FormatOptions Format = FormatOptions.Java;

        public static CompoundTag ToCompound(this Stream stream)
        {
            using var reader = new TagReader(stream, Format, true);
            var compound = (CompoundTag) reader.ReadTag();
            return compound;
        }

        public static byte[] ToBytes(this CompoundTag tag)
        {
            using var mem = new MemoryStream();
            using var writer = new TagWriter(mem, Format);
            writer.WriteCompound(tag);
            return mem.ToArray();
        }

        public static byte[] ToBytes(INbtSerializable obj)
        {
            var tag = obj.ToCompound();
            return ToBytes(tag);
        }
    }
}