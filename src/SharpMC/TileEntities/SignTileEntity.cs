using SharpNBT;

namespace SharpMC.TileEntities
{
    public class SignTileEntity : TileEntity
    {
        public const string NameId = "Sign";

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string Line4 { get; set; }

        public SignTileEntity() : base(NameId)
        {
            Line1 = string.Empty;
            Line2 = string.Empty;
            Line3 = string.Empty;
            Line4 = string.Empty;
        }

        public override CompoundTag GetCompound(string name = null)
        {
            Line1 ??= string.Empty;
            Line2 ??= string.Empty;
            Line3 ??= string.Empty;
            Line4 ??= string.Empty;

            var compound = new CompoundTag(string.Empty)
            {
                new StringTag("id", Id),
                new StringTag("Text1", Line1),
                new StringTag("Text2", Line2),
                new StringTag("Text3", Line3),
                new StringTag("Text4", Line4),
                new IntTag("x", (int) Coordinates.X),
                new IntTag("y", (int) Coordinates.Y),
                new IntTag("z", (int) Coordinates.Z)
            };

            return compound;
        }

        public override void SetCompound(CompoundTag compound)
        {
            Line1 = GetTextValue(compound, "Text1");
            Line2 = GetTextValue(compound, "Text2");
            Line3 = GetTextValue(compound, "Text3");
            Line4 = GetTextValue(compound, "Text4");
        }
    }
}