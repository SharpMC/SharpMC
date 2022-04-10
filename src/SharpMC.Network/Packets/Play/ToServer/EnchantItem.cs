using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class EnchantItem : Packet<EnchantItem>, IToServer
    {
        public byte ServerId => 0x07;

        public sbyte WindowId { get; set; }
        public sbyte Enchantment { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadSByte();
            Enchantment = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(WindowId);
            stream.WriteSByte(Enchantment);
        }
    }
}
