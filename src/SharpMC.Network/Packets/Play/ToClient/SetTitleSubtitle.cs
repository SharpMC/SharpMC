using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SetTitleSubtitle : Packet<SetTitleSubtitle>, IToClient
    {
        public byte ClientId => 0x58;

        public string Text { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Text = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Text);
        }
    }
}
