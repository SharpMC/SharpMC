using System;
using System.Numerics;

namespace SharpMC.Network.Util
{
    public interface IMinecraftReader
    {
        string ReadString();
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
        byte[] ReadNbt();
        byte[] Read(int length);
    }
}