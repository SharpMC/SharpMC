using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class ScoreboardDisplayObjective : Packet<ScoreboardDisplayObjective>, IToClient
    {
        public byte ClientId => 0x4c;

        public sbyte Position { get; set; }
        public string Name { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Position = stream.ReadSByte();
            Name = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Position);
            stream.WriteString(Name);
        }
    }
}
