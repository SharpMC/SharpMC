using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Tags : Packet<Tags>, IToClient
    {
        public byte ClientId => 0x67;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
