using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class KickDisconnect : Packet<KickDisconnect>, IToClient
    {
        public byte ClientId => 0x1a;

        public string Reason { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Reason = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Reason);
        }
    }
}
