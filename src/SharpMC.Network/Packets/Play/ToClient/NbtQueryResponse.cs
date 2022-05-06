using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class NbtQueryResponse : Packet<NbtQueryResponse>, IToClient
    {
        public byte ClientId => 0x60;

        public int TransactionId { get; set; }
        public object Nbt { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            TransactionId = stream.ReadVarInt();
            Nbt = stream.ReadOptNbt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteOptNbt(Nbt);
        }
    }
}
