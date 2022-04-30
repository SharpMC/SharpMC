using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class CraftRecipeResponse : Packet<CraftRecipeResponse>, IToClient
    {
        public byte ClientId => 0x31;

        public sbyte WindowId { get; set; }
        public string Recipe { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadSByte();
            Recipe = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(WindowId);
            stream.WriteString(Recipe);
        }
    }
}
