using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class HeldItemSlot : Packet<HeldItemSlot>, IToClient
    {
        public byte ClientId => 0x48;

        public sbyte Slot { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Slot = stream.ReadSByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(Slot);
        }
    }
}
