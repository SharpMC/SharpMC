using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToBoth
{
    public class KeepAlive : Packet<KeepAlive>, IToClient, IToServer
    {
        public byte ClientId => 0x21;
        public byte ServerId => 0x0f;

        public long KeepAliveId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            KeepAliveId = stream.ReadLong();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteLong(KeepAliveId);
        }
    }
}
