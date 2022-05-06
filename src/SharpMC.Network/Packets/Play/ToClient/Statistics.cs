using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Statistics : Packet<Statistics>, IToClient
    {
        public byte ClientId => 0x07;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
