using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityMoveLook : Packet<EntityMoveLook>, IToClient
    {
        public byte ClientId => 0x2a;

        public int EntityId { get; set; }
        public short DX { get; set; }
        public short DY { get; set; }
        public short DZ { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            DX = stream.ReadShort();
            DY = stream.ReadShort();
            DZ = stream.ReadShort();
            Yaw = stream.ReadSByte();
            Pitch = stream.ReadSByte();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteShort(DX);
            stream.WriteShort(DY);
            stream.WriteShort(DZ);
            stream.WriteSByte(Yaw);
            stream.WriteSByte(Pitch);
            stream.WriteBool(OnGround);
        }
    }
}
