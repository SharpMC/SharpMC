using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class BlockChange : Packet<BlockChange>, IToClient
    {
        public byte ClientId => 0x0c;

        public Vector3 Location { get; set; }
        public int Type { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Type = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteVarInt(Type);
        }
    }
}
