using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Teams : Packet<Teams>, IToClient
    {
        public byte ClientId => 0x55;

        public string Team { get; set; }
        public sbyte Mode { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Team = stream.ReadString();
            Mode = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Team);
            stream.WriteSByte(Mode);
        }
    }
}
