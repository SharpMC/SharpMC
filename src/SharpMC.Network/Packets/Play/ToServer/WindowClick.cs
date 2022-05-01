using SharpMC.Network.API;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class WindowClick : Packet<WindowClick>, IToServer
    {
        public byte ServerId => 0x08;

        public byte WindowId { get; set; }
        public int StateId { get; set; }
        public short Slot { get; set; }
        public sbyte MouseButton { get; set; }
        public int Mode { get; set; }
        public SlotData CursorItem { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadByte();
            StateId = stream.ReadVarInt();
            Slot = stream.ReadShort();
            MouseButton = stream.ReadSByte();
            Mode = stream.ReadVarInt();
            CursorItem = (SlotData) stream.ReadSlot();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(WindowId);
            stream.WriteVarInt(StateId);
            stream.WriteShort(Slot);
            stream.WriteSByte(MouseButton);
            stream.WriteVarInt(Mode);
            stream.WriteSlot(CursorItem);
        }
    }
}
