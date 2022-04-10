using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class CraftRecipeRequest : Packet<CraftRecipeRequest>, IToServer
    {
        public byte ServerId => 0x18;

        public sbyte WindowId { get; set; }
        public string Recipe { get; set; }
        public bool MakeAll { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadSByte();
            Recipe = stream.ReadString();
            MakeAll = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteSByte(WindowId);
            stream.WriteString(Recipe);
            stream.WriteBool(MakeAll);
        }
    }
}
