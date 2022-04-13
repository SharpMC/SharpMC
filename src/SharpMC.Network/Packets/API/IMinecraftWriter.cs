using System;
using System.Numerics;
using SharpMC.Network.Binary;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets;

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
        void WriteSlot(SlotData value);
        void WriteFloat(float value);
        void WriteUuid(Guid value);
        void WritePosition(Vector3 value);
        void WriteUShort(ushort value);
        void WriteDouble(double value);
        void WriteInt(int value);
        void WriteBuffer(byte[] data);
        void WriteByteArray(byte[] values);
        void WriteByteArrays(byte[][] values);
        void WriteLong(long value);
        void WriteLongArray(long[] values);
        void WriteOptNbt(object data);
        void WriteNbt(INbtSerializable data);
        void WriteMetadata(byte[] data);
        void WriteBitField(IPacket value);
        void WriteBitFieldArray<T>(T[] entities) where T : IPacket, new();
        void Write(byte[] data);
    }
}