using System;
using System.Numerics;

namespace SharpMC.Network.Util
{
    public interface IMinecraftWriter
    {
        void WriteString(string text);
        void WriteStringArray(string[] texts);
        void WriteSByte(sbyte value);
        void WriteVarInt(int value);
        void WriteBool(bool value);
        void WriteByte(byte value);
        void WriteShort(short value);
        void WriteSlot(byte value);
        void WriteFloat(float value);
        void WriteUuid(Guid value);
        void WritePosition(Vector3 value);
        void WriteUShort(ushort value);
        void WriteDouble(double value);
        void WriteInt(int value);
        void WriteBuffer(byte[] data);
        void WriteLong(long value);
        void WriteOptNbt(byte[] data);
        void WriteNbt(byte[] data);
        void WriteMetadata(byte[] data);
        void Write(byte[] data);
    }
}