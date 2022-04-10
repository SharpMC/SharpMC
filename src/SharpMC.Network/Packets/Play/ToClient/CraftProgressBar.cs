using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class CraftProgressBar : Packet<CraftProgressBar>, IToClient
    {
        public byte ClientId => 0x15;

        public byte WindowId { get; set; }
        public short Property { get; set; }
        public short Value { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadByte();
            Property = stream.ReadShort();
            Value = stream.ReadShort();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(WindowId);
            stream.WriteShort(Property);
            stream.WriteShort(Value);
        }
    }
}
