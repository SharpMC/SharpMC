using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class PickItem : Packet<PickItem>, IToServer
    {
        public byte ServerId => 0x17;

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
