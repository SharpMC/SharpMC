using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class DisplayedRecipe : Packet<DisplayedRecipe>, IToServer
    {
        public byte ServerId => 0x1f;

        public string RecipeId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            RecipeId = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(RecipeId);
        }
    }
}
