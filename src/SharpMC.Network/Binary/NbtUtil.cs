using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public static class NbtUtil
    {
        private const FormatOptions Format = FormatOptions.Java;

        public static CompoundTag ToCompound(this Stream stream)
        {
            using var reader = new TagReader(stream, Format, true);
            var raw = reader.ReadTag();
            var compound = raw is EndTag ? null : (CompoundTag)raw;
            return compound;
        }

        public static byte[] ToBytes(this CompoundTag tag)
        {
            using var mem = new MemoryStream();
            using var writer = new TagWriter(mem, Format);
            writer.WriteCompound(tag);
            return mem.ToArray();
        }

        public static byte[] ToBytes(this INbtSerializable obj)
        {
            var tag = obj.ToCompound();
            return ToBytes(tag);
        }

        public static T ToObject<T>(this CompoundTag tag) where T : INbtSerializable, new()
        {
            var obj = new T();
            obj.ToObject(tag);
            return obj;
        }

        public static Dictionary<string, object> ToDict(CompoundTag tag)
        {
            return tag.ToDictionary(k => k.Name, ToValue);
        }

        private static object ToValue(Tag tag)
        {
            var kind = tag.Type;
            switch (kind)
            {
                case TagType.Byte: return ((ByteTag) tag).Value;
                case TagType.Int: return ((IntTag) tag).Value;
                case TagType.Float: return ((FloatTag) tag).Value;
                case TagType.Double: return ((DoubleTag) tag).Value;
                case TagType.String: return ((StringTag) tag).Value;
                default: throw new InvalidOperationException($"{kind} ?!");
            }
        }
    }
}