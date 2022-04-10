using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityStatus : Packet<EntityStatus>, IToClient
    {
        public byte ClientId => 0x1b;

        public int EntityId { get; set; }
        public sbyte _EntityStatus { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadInt();
            _EntityStatus = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(EntityId);
            stream.WriteSByte(_EntityStatus);
        }
    }
}
