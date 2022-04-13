using System;
using System.Numerics;
using SharpMC.Network.Binary;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets;

namespace SharpMC.Network.Util
{
    public interface IMinecraftReader
    {
        string ReadString();
        string[] ReadStringArray();
        sbyte ReadSByte();
        int ReadVarInt();
        bool ReadBool();
        byte ReadByte();
        short ReadShort();
        SlotData ReadSlot();
        float ReadFloat();
        Guid ReadUuid();
        Vector3 ReadPosition();
        ushort ReadUShort();
        int ReadInt();
        double ReadDouble();
        byte[] ReadBuffer();
        byte[] ReadByteArray();
        byte[][] ReadByteArrays();
        long ReadLong();
        long[] ReadLongArray();
        byte[] ReadMetadata();
        object ReadOptNbt();
        T ReadNbt<T>() where T : INbtSerializable, new();
        T ReadBitField<T>() where T : IPacket, new();
        T[] ReadBitFieldArray<T>() where T : IPacket, new();
        byte[] Read(int length);
    }
}