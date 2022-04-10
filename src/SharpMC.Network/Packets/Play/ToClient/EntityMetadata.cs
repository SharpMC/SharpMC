using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityMetadata : Packet<EntityMetadata>, IToClient
    {
        public byte ClientId => 0x4d;

        public int EntityId { get; set; }
        public byte[] Metadata { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            Metadata = stream.ReadMetadata();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteMetadata(Metadata);
        }
    }
}
