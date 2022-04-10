using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SetTitleTime : Packet<SetTitleTime>, IToClient
    {
        public byte ClientId => 0x5b;

        public int FadeIn { get; set; }
        public int Stay { get; set; }
        public int FadeOut { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            FadeIn = stream.ReadInt();
            Stay = stream.ReadInt();
            FadeOut = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteInt(FadeIn);
            stream.WriteInt(Stay);
            stream.WriteInt(FadeOut);
        }
    }
}
