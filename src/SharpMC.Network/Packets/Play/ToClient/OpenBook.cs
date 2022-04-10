using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class OpenBook : Packet<OpenBook>, IToClient
    {
        public byte ClientId => 0x2d;

        public int Hand { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Hand = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Hand);
        }
    }
}
