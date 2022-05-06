using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class BlockDig : Packet<BlockDig>, IToServer
    {
        public byte ServerId => 0x1a;

        public sbyte Status { get; set; }
        public Vector3 Location { get; set; }
        public sbyte Face { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Status = stream.ReadSByte();
            Location = stream.ReadPosition();
            Face = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Status);
            stream.WritePosition(Location);
            stream.WriteSByte(Face);
        }
    }
}
