using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Advancements : Packet<Advancements>, IToClient
    {
        public byte ClientId => 0x63;

        public bool Reset { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Reset = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(Reset);
        }
    }
}
