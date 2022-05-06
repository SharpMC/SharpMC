using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class OpenHorseWindow : Packet<OpenHorseWindow>, IToClient
    {
        public byte ClientId => 0x1f;

        public byte WindowId { get; set; }
        public int NbSlots { get; set; }
        public int EntityId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadByte();
            NbSlots = stream.ReadVarInt();
            EntityId = stream.ReadInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(WindowId);
            stream.WriteVarInt(NbSlots);
            stream.WriteInt(EntityId);
        }
    }
}
