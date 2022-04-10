using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Position : Packet<Position>, IToServer
    {
        public byte ServerId => 0x11;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteBool(OnGround);
        }
    }
}
