using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class RelEntityMove : Packet<RelEntityMove>, IToClient
    {
        public byte ClientId => 0x29;

        public int EntityId { get; set; }
        public short DX { get; set; }
        public short DY { get; set; }
        public short DZ { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            DX = stream.ReadShort();
            DY = stream.ReadShort();
            DZ = stream.ReadShort();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteShort(DX);
            stream.WriteShort(DY);
            stream.WriteShort(DZ);
            stream.WriteBool(OnGround);
        }
    }
}
