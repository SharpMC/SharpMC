using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class GameStateChange : Packet<GameStateChange>, IToClient
    {
        public byte ClientId => 0x1e;

        public byte Reason { get; set; }
        public float GameMode { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Reason = stream.ReadByte();
            GameMode = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(Reason);
            stream.WriteFloat(GameMode);
        }
    }
}
