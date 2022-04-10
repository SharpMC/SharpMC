using System;
using System.Numerics;

namespace SharpMC.Generator.Prismarine
{
    internal static class FieldMapping
    {
        public static Type GetNative(string type)
        {
            switch (type)
            {
                case "varint": return typeof(int);
                case "string": return typeof(string);
                case "bool": return typeof(bool);
                case "u8": return typeof(byte);
                case "i8": return typeof(sbyte);
                case "u16": return typeof(ushort);
                case "i16": return typeof(short);
                case "i32": return typeof(int);
                case "i64": return typeof(long);
                case "f32": return typeof(float);
                case "f64": return typeof(double);
                case "UUID": return typeof(Guid);
                case "slot": return typeof(byte);
                case "restBuffer": return typeof(byte[]);
                case "optionalNbt": return typeof(byte[]);
                case "entityMetadata": return typeof(byte[]);
                case "nbt": return typeof(byte[]);
                case "position": return typeof(Vector3);
                default: throw new InvalidOperationException(type);
            }
        }

        public static string GetShort(string text)
        {
            switch (text)
            {
                case "System.String": return "string";
                case "System.Boolean": return "bool";
                case "System.Int16": return "short";
                case "System.Int32": return "int";
                case "System.Int64": return "long";
                case "System.Single": return "float";
                case "System.Double": return "double";
                case "System.SByte": return "sbyte";
                case "System.Byte": return "byte";
                case "System.Byte[]": return "byte[]";
                case "System.Guid": return "Guid";
                case "System.UInt16": return "ushort";
                default: return text;
            }
        }
    }
}