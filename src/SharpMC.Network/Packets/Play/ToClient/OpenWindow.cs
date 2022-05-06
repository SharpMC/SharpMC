using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class OpenWindow : Packet<OpenWindow>, IToClient
    {
        public byte ClientId => 0x2e;

        public int WindowId { get; set; }
        public int InventoryType { get; set; }
        public string WindowTitle { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadVarInt();
            InventoryType = stream.ReadVarInt();
            WindowTitle = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(WindowId);
            stream.WriteVarInt(InventoryType);
            stream.WriteString(WindowTitle);
        }
    }
}
