using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class PlayerlistHeader : Packet<PlayerlistHeader>, IToClient
    {
        public byte ClientId => 0x5f;

        public string Header { get; set; }
        public string Footer { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Header = stream.ReadString();
            Footer = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Header);
            stream.WriteString(Footer);
        }
    }
}
