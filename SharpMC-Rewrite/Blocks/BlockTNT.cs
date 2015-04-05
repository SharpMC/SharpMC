using SharpMCRewrite.Classes;

namespace SharpMCRewrite.Blocks
{
	internal class BlockTNT : Block
	{
		internal BlockTNT() : base(46)
		{
			IsSolid = true;
			IsReplacible = false;
		}

		public override void BreakBlock(Level world)
		{
			world.SetBlock(new BlockAir {Coordinates = Coordinates});
			new Explosion(world, Coordinates, 4f).Explode();
		}
	}
}