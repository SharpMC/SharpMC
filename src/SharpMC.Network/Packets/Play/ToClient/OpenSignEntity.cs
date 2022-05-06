using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class OpenSignEntity : Packet<OpenSignEntity>, IToClient
    {
        public byte ClientId => 0x2f;

        public Vector3 Location { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
        }
    }
}
