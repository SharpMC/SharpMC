using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UseItem : Packet<UseItem>, IToServer
    {
        public byte ServerId => 0x2f;

        public int Hand { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Hand = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Hand);
        }
    }
}
