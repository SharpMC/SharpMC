using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class PlayerInfo : Packet<PlayerInfo>, IToClient
    {
        public byte ClientId => 0x36;

        public int Action { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Action = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Action);
        }
    }
}
