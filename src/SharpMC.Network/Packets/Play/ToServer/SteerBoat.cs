using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SteerBoat : Packet<SteerBoat>, IToServer
    {
        public byte ServerId => 0x16;

        public bool LeftPaddle { get; set; }
        public bool RightPaddle { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            LeftPaddle = stream.ReadBool();
            RightPaddle = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(LeftPaddle);
            stream.WriteBool(RightPaddle);
        }
    }
}
