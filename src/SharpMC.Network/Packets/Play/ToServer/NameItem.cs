using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class NameItem : Packet<NameItem>, IToServer
    {
        public byte ServerId => 0x20;

        public string Name { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Name = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Name);
        }
    }
}
