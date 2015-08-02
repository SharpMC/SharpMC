using fNbt;

namespace SharpMC.Core.TileEntities
{
	public class SignTileEntity : TileEntity
	{
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }
		public string Line4 { get; set; }

		public SignTileEntity() : base("Sign")
		{
			Line1 = string.Empty;
			Line2 = string.Empty;
			Line3 = string.Empty;
			Line4 = string.Empty;
		}

		public override NbtCompound GetCompound()
		{
			if (Line1 == null) Line1 = string.Empty;
			if (Line2 == null) Line2 = string.Empty;
			if (Line3 == null) Line3 = string.Empty;
			if (Line4 == null) Line4 = string.Empty;

			var compound = new NbtCompound(string.Empty)
			{
				new NbtString("id", Id),
				new NbtString("Text1", Line1),
				new NbtString("Text2", Line2),
				new NbtString("Text3", Line3),
				new NbtString("Text4", Line4),
				new NbtInt("x", (int)Coordinates.X),
				new NbtInt("y", (int)Coordinates.Y),
				new NbtInt("z", (int)Coordinates.Z)
			};

			return compound;
		}

		public override void SetCompound(NbtCompound compound)
		{
			Line1 = GetTextValue(compound, "Text1");
			Line2 = GetTextValue(compound, "Text2");
			Line3 = GetTextValue(compound, "Text3");
			Line4 = GetTextValue(compound, "Text4");
		}

		private string GetTextValue(NbtCompound compound, string key)
		{
			NbtString text;
			compound.TryGet(key, out text);
			return text != null ? (text.StringValue ?? string.Empty) : string.Empty;
		}
	}
}
