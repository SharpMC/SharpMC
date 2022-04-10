using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SelectAdvancementTab : Packet<SelectAdvancementTab>, IToClient
    {
        public byte ClientId => 0x40;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
