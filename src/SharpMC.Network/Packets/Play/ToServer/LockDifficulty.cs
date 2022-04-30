using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class LockDifficulty : Packet<LockDifficulty>, IToServer
    {
        public byte ServerId => 0x10;

        public bool Locked { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Locked = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(Locked);
        }
    }
}
