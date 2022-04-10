using SharpMC.Network.Util;
using System;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SpawnEntity : Packet<SpawnEntity>, IToClient
    {
        public byte ClientId => 0x00;

        public int EntityId { get; set; }
        public Guid ObjectUUID { get; set; }
        public int Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Pitch { get; set; }
        public sbyte Yaw { get; set; }
        public int ObjectData { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            ObjectUUID = stream.ReadUuid();
            Type = stream.ReadVarInt();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Pitch = stream.ReadSByte();
            Yaw = stream.ReadSByte();
            ObjectData = stream.ReadInt();
            VelocityX = stream.ReadShort();
            VelocityY = stream.ReadShort();
            VelocityZ = stream.ReadShort();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteUuid(ObjectUUID);
            stream.WriteVarInt(Type);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteSByte(Pitch);
            stream.WriteSByte(Yaw);
            stream.WriteInt(ObjectData);
            stream.WriteShort(VelocityX);
            stream.WriteShort(VelocityY);
            stream.WriteShort(VelocityZ);
        }
    }
}
