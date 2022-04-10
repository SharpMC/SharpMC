using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class ScoreboardScore : Packet<ScoreboardScore>, IToClient
    {
        public byte ClientId => 0x56;

        public string ItemName { get; set; }
        public sbyte Action { get; set; }
        public string ScoreName { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ItemName = stream.ReadString();
            Action = stream.ReadSByte();
            ScoreName = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(ItemName);
            stream.WriteSByte(Action);
            stream.WriteString(ScoreName);
        }
    }
}
