using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class QueryEntityNbt : Packet<QueryEntityNbt>, IToServer
    {
        public byte ServerId => 0x0c;

        public int TransactionId { get; set; }
        public int EntityId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            TransactionId = stream.ReadVarInt();
            EntityId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteVarInt(EntityId);
        }
    }
}
