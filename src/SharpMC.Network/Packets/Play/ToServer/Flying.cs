using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Flying : Packet<Flying>, IToServer
    {
        public byte ServerId => 0x14;

        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(OnGround);
        }
    }
}
