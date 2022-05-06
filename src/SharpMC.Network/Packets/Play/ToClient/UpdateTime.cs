using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UpdateTime : Packet<UpdateTime>, IToClient
    {
        public byte ClientId => 0x59;

        public long Age { get; set; }
        public long Time { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Age = stream.ReadLong();
            Time = stream.ReadLong();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteLong(Age);
            stream.WriteLong(Time);
        }
    }
}
