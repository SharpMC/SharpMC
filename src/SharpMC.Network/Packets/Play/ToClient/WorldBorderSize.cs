using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldBorderSize : Packet<WorldBorderSize>, IToClient
    {
        public byte ClientId => 0x44;

        public double Diameter { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Diameter = stream.ReadDouble();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(Diameter);
        }
    }
}
