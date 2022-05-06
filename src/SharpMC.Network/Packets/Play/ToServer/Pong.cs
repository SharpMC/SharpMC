using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Pong : Packet<Pong>, IToServer
    {
        public byte ServerId => 0x1d;

        public int Id { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Id = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(Id);
        }
    }
}
