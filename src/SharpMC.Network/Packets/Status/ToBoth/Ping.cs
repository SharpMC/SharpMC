using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Status.ToBoth
{
    public class Ping : Packet<Ping>, IToClient, IToServer
    {
        public byte ClientId => 0x01;
        public byte ServerId => 0x01;

        public long Time { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Time = stream.ReadLong();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteLong(Time);
        }
    }
}
