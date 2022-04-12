using System;
using System.Numerics;
using SharpMC.Network.Binary;
using SharpMC.Network.Binary.Special;

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
        long ReadLong();
        byte[] ReadMetadata();
        object ReadOptNbt();
        T ReadNbt<T>() where T : INbtSerializable, new();
        byte[] Read(int length);
    }
}