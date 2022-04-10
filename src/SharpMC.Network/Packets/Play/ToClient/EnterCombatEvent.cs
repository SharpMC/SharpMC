using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class EnterCombatEvent : Packet<EnterCombatEvent>, IToClient
    {
        public byte ClientId => 0x34;


        public override void Decode(IMinecraftStream stream)
        {
        }

        public override void Encode(IMinecraftStream stream)
        {
        }
    }
}
