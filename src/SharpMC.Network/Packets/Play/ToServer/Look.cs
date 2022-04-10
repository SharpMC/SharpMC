using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Look : Packet<Look>, IToServer
    {
        public byte ServerId => 0x13;

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Yaw = stream.ReadFloat();
            Pitch = stream.ReadFloat();
            OnGround = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteFloat(Yaw);
            stream.WriteFloat(Pitch);
            stream.WriteBool(OnGround);
        }
    }
}
