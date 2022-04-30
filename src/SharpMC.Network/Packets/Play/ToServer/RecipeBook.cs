using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class RecipeBook : Packet<RecipeBook>, IToServer
    {
        public byte ServerId => 0x1e;

        public int BookId { get; set; }
        public bool BookOpen { get; set; }
        public bool FilterActive { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            BookId = stream.ReadVarInt();
            BookOpen = stream.ReadBool();
            FilterActive = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(BookId);
            stream.WriteBool(BookOpen);
            stream.WriteBool(FilterActive);
        }
    }
}
