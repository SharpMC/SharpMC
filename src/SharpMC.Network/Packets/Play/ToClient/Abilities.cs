using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Abilities : Packet<Abilities>, IToClient
    {
        public byte ClientId => 0x32;

        public sbyte Flags { get; set; }
        public float FlyingSpeed { get; set; }
        public float WalkingSpeed { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Flags = stream.ReadSByte();
            FlyingSpeed = stream.ReadFloat();
            WalkingSpeed = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Flags);
            stream.WriteFloat(FlyingSpeed);
            stream.WriteFloat(WalkingSpeed);
        }
    }
}
