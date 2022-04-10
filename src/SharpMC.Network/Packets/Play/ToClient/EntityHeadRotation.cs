using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityHeadRotation : Packet<EntityHeadRotation>, IToClient
    {
        public byte ClientId => 0x3e;

        public int EntityId { get; set; }
        public sbyte HeadYaw { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            HeadYaw = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteSByte(HeadYaw);
        }
    }
}
