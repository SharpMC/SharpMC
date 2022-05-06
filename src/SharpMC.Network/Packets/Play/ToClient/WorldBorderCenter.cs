using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WorldBorderCenter : Packet<WorldBorderCenter>, IToClient
    {
        public byte ClientId => 0x42;

        public double X { get; set; }
        public double Z { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Z = stream.ReadDouble();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Z);
        }
    }
}
