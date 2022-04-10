using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SelectTrade : Packet<SelectTrade>, IToServer
    {
        public byte ServerId => 0x23;

        public int Slot { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Slot = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Slot);
        }
    }
}
