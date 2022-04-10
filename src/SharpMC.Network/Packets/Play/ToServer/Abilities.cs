using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Abilities : Packet<Abilities>, IToServer
    {
        public byte ServerId => 0x19;

        public sbyte Flags { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Flags = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Flags);
        }
    }
}
