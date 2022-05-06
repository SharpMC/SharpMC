using SharpMC.Network.Packets.API;
using System;
using SharpMC.Network.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SpawnEntityLiving : Packet<SpawnEntityLiving>, IToClient
    {
        public byte ClientId => 0x02;

        public int EntityId { get; set; }
        public Guid EntityUUID { get; set; }
        public int Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public sbyte HeadPitch { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            EntityUUID = stream.ReadUuid();
            Type = stream.ReadVarInt();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSByte();
            Pitch = stream.ReadSByte();
            HeadPitch = stream.ReadSByte();
            VelocityX = stream.ReadShort();
            VelocityY = stream.ReadShort();
            VelocityZ = stream.ReadShort();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteUuid(EntityUUID);
            stream.WriteVarInt(Type);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteSByte(Yaw);
            stream.WriteSByte(Pitch);
            stream.WriteSByte(HeadPitch);
            stream.WriteShort(VelocityX);
            stream.WriteShort(VelocityY);
            stream.WriteShort(VelocityZ);
        }
    }
}
