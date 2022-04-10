using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class TabComplete : Packet<TabComplete>, IToClient
    {
        public byte ClientId => 0x11;

        public int TransactionId { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            TransactionId = stream.ReadVarInt();
            Start = stream.ReadVarInt();
            Length = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteVarInt(Start);
            stream.WriteVarInt(Length);
        }
    }
}
