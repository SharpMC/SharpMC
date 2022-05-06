using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class Compress : Packet<Compress>, IToClient
    {
        public byte ClientId => 0x03;

        public int Threshold { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Threshold = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Threshold);
        }
    }
}
