using System;

namespace SharpMC.Generator.Prismarine
{
    internal static class MethodMapping
    {
        public static string GetWriter(string type, string fieldName)
        {
            switch (type)
            {
                case "entityMetadata": return $"WriteMetadata({fieldName})";
                case "bool": return $"WriteBool({fieldName})";
                case "f32": return $"WriteFloat({fieldName})";
                case "f64": return $"WriteDouble({fieldName})";
                case "i8": return $"WriteSByte({fieldName})";
                case "i16": return $"WriteShort({fieldName})";
                case "i32": return $"WriteInt({fieldName})";
                case "i64": return $"WriteLong({fieldName})";
                case "u8": return $"WriteByte({fieldName})";
                case "u16": return $"WriteUShort({fieldName})";
                case "string": return $"WriteString({fieldName})";
                case "nbt": return $"WriteNbt({fieldName})";
                case "optionalNbt": return $"WriteOptNbt({fieldName})";
                case "position": return $"WritePosition({fieldName})";
                case "restBuffer": return $"WriteBuffer({fieldName})";
                case "slot": return $"WriteSlot({fieldName})";
                case "UUID": return $"WriteUuid({fieldName})";
                case "varint": return $"WriteVarInt({fieldName})";
                default: throw new InvalidOperationException(type);
            }
        }

        public static string GetReader(string type)
        {
            switch (type)
            {
                case "entityMetadata": return "ReadMetadata()";
                case "bool": return "ReadBool()";
                case "f32": return "ReadFloat()";
                case "f64": return "ReadDouble()";
                case "i8": return "ReadSByte()";
                case "i16": return "ReadShort()";
                case "i32": return "ReadInt()";
                case "i64": return "ReadLong()";
                case "u8": return "ReadByte()";
                case "u16": return "ReadUShort()";
                case "string": return "ReadString()";
                case "nbt": return "ReadNbt()";
                case "optionalNbt": return "ReadOptNbt()";
                case "position": return "ReadPosition()";
                case "restBuffer": return "ReadBuffer()";
                case "slot": return "ReadSlot()";
                case "UUID": return "ReadUuid()";
                case "varint": return "ReadVarInt()";
                default: throw new InvalidOperationException(type);
            }
        }
    }
}