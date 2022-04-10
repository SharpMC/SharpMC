using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityEquipment : Packet<EntityEquipment>, IToClient
    {
        public byte ClientId => 0x50;

        public int EntityId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
        }
    }
}
