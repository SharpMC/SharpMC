using System;
using System.Numerics;
using SharpMC.Network.Binary;

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
        byte ReadSlot();
        float ReadFloat();
        Guid ReadUuid();
        Vector3 ReadPosition();
        ushort ReadUShort();
        int ReadInt();
        double ReadDouble();
        byte[] ReadBuffer();
        long ReadLong();
        byte[] ReadMetadata();
        byte[] ReadOptNbt();
        T ReadNbt<T>() where T : INbtSerializable, new();
        byte[] Read(int length);
    }
}