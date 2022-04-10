using SharpMC.Network.Util;
using System;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class NamedEntitySpawn : Packet<NamedEntitySpawn>, IToClient
    {
        public byte ClientId => 0x04;

        public int EntityId { get; set; }
        public Guid PlayerUUID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            PlayerUUID = stream.ReadUuid();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSByte();
            Pitch = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteUuid(PlayerUUID);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteSByte(Yaw);
            stream.WriteSByte(Pitch);
        }
    }
}
