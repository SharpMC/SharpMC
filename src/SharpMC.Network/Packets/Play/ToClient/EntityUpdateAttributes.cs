using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityUpdateAttributes : Packet<EntityUpdateAttributes>, IToClient
    {
        public byte ClientId => 0x64;

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
