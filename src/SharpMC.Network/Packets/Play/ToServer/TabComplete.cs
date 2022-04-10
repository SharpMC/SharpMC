using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class TabComplete : Packet<TabComplete>, IToServer
    {
        public byte ServerId => 0x06;

        public int TransactionId { get; set; }
        public string Text { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            TransactionId = stream.ReadVarInt();
            Text = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteString(Text);
        }
    }
}
