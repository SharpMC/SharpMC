using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class MultiBlockChange : Packet<MultiBlockChange>, IToClient
    {
        public byte ClientId => 0x3f;

        public bool NotTrustEdges { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            NotTrustEdges = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(NotTrustEdges);
        }
    }
}
