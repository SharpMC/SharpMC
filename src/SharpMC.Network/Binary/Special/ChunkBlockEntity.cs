using System;
using System.Collections.Generic;
using System.Text;
using SharpMC.Network.Packets;
using SharpMC.Network.Util;

namespace SharpMC.Network.Binary.Special
{
    public class ChunkBlockEntity : IPacket
    {
        public PackedCoordinates Coordinates { get; set; }
        public short Y { get; set; }
        public int Type { get; set; }
        public object Optional { get; set; }

        public void Encode(IMinecraftStream stream)
        {
            stream.WriteBitField(Coordinates);
            stream.WriteShort(Y);
            stream.WriteVarInt(Type);
            stream.WriteOptNbt(Optional);
        }

        public void Decode(IMinecraftStream stream)
        {
            Coordinates = stream.ReadBitField<PackedCoordinates>();
            Y = stream.ReadShort();
            Type = stream.ReadVarInt();
            Optional = stream.ReadOptNbt();
        }
    }
}