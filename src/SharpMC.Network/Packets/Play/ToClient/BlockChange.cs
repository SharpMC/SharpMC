using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class BlockChange : Packet<BlockChange>, IToClient
    {
        public byte ClientId => 0x0c;

        public System.Numerics.Vector3 Location { get; set; }
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
