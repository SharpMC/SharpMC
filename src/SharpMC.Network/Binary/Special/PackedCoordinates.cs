using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Binary.Special
{
    public class PackedCoordinates : IPacket
    {
        public int BlockX { get; set; }
        public int BlockZ { get; set; }

        public void Encode(IMinecraftStream stream)
        {
            var code = ((BlockX & 15) << 4) | (BlockZ & 15);
            var tmp = (byte) code;
            stream.WriteByte(tmp);
        }

        public void Decode(IMinecraftStream stream)
        {
            var code = stream.ReadByte();
            BlockX = code >> 4;
            BlockZ = code ^ ((code >> 4) << 4);
        }
    }
}