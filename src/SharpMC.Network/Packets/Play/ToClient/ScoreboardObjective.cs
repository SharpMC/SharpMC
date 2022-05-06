using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class ScoreboardObjective : Packet<ScoreboardObjective>, IToClient
    {
        public byte ClientId => 0x53;

        public string Name { get; set; }
        public sbyte Action { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Name = stream.ReadString();
            Action = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Name);
            stream.WriteSByte(Action);
        }
    }
}
