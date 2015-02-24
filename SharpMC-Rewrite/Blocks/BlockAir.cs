namespace SharpMCRewrite.Blocks
{
	internal class BlockAir : Block
	{
		internal BlockAir() : base(0)
		{
			IsReplacible = true;
			IsSolid = false;
		}
	}
}