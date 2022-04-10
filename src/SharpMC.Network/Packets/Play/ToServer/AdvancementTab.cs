using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class AdvancementTab : Packet<AdvancementTab>, IToServer
    {
        public byte ServerId => 0x22;

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
