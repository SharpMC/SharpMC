using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class HeldItemSlot : Packet<HeldItemSlot>, IToServer
    {
        public byte ServerId => 0x25;

        public short SlotId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            SlotId = stream.ReadShort();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteShort(SlotId);
        }
    }
}
