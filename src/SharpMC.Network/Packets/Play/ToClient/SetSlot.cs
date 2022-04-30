using SharpMC.Network.API;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SetSlot : Packet<SetSlot>, IToClient
    {
        public byte ClientId => 0x16;

        public sbyte WindowId { get; set; }
        public int StateId { get; set; }
        public short Slot { get; set; }
        public SlotData Item { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadSByte();
            StateId = stream.ReadVarInt();
            Slot = stream.ReadShort();
            Item = stream.ReadSlot();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(WindowId);
            stream.WriteVarInt(StateId);
            stream.WriteShort(Slot);
            stream.WriteSlot(Item);
        }
    }
}
