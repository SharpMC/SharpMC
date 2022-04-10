using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Status.ToServer
{
    public class PingStart : Packet<PingStart>, IToServer
    {
        public byte ServerId => 0x00;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
