using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Difficulty : Packet<Difficulty>, IToClient
    {
        public byte ClientId => 0x0e;

        public byte _Difficulty { get; set; }
        public bool DifficultyLocked { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            _Difficulty = stream.ReadByte();
            DifficultyLocked = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(_Difficulty);
            stream.WriteBool(DifficultyLocked);
        }
    }
}
