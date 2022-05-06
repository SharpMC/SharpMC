using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EntityDestroy : Packet<EntityDestroy>, IToClient
    {
        public byte ClientId => 0x3a;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
