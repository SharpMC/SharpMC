using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityTeleport : Packet<EntityTeleport>, IToClient
    {
        public byte ClientId => 0x62;

        public int EntityId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            Yaw = stream.ReadSByte();
            Pitch = stream.ReadSByte();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteSByte(Yaw);
            stream.WriteSByte(Pitch);
            stream.WriteBool(OnGround);
        }
    }
}
