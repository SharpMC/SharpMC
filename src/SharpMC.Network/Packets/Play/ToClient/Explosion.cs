using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Explosion : Packet<Explosion>, IToClient
    {
        public byte ClientId => 0x1c;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }
        public float PlayerMotionX { get; set; }
        public float PlayerMotionY { get; set; }
        public float PlayerMotionZ { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            X = stream.ReadFloat();
            Y = stream.ReadFloat();
            Z = stream.ReadFloat();
            Radius = stream.ReadFloat();
            PlayerMotionX = stream.ReadFloat();
            PlayerMotionY = stream.ReadFloat();
            PlayerMotionZ = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteFloat(X);
            stream.WriteFloat(Y);
            stream.WriteFloat(Z);
            stream.WriteFloat(Radius);
            stream.WriteFloat(PlayerMotionX);
            stream.WriteFloat(PlayerMotionY);
            stream.WriteFloat(PlayerMotionZ);
        }
    }
}
