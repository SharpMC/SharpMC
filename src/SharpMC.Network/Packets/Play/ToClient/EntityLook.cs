using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityLook : Packet<EntityLook>, IToClient
    {
        public byte ClientId => 0x2b;

        public int EntityId { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            Yaw = stream.ReadSByte();
            Pitch = stream.ReadSByte();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteSByte(Yaw);
            stream.WriteSByte(Pitch);
            stream.WriteBool(OnGround);
        }
    }
}
