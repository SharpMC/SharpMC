using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class SetCreativeSlot : Packet<SetCreativeSlot>, IToServer
    {
        public byte ServerId => 0x28;

        public short Slot { get; set; }
        public byte Item { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Slot = stream.ReadShort();
            Item = stream.ReadSlot();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteShort(Slot);
            stream.WriteSlot(Item);
        }
    }
}
