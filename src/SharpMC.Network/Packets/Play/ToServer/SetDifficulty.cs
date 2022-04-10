using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SetDifficulty : Packet<SetDifficulty>, IToServer
    {
        public byte ServerId => 0x02;

        public byte NewDifficulty { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            NewDifficulty = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(NewDifficulty);
        }
    }
}
