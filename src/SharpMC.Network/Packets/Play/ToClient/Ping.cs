using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Ping : Packet<Ping>, IToClient
    {
        public byte ClientId => 0x30;

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
