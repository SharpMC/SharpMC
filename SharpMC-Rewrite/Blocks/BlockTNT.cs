using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	class BlockTNT : Block
	{
		internal BlockTNT() : base(46)
		{
			IsSolid = true;
			IsReplacible = false;
		}

		public override void BreakBlock(Level world)
		{
			world.SetBlock(new BlockAir() {Coordinates = Coordinates});
			new Explosion(world, Coordinates, 4f).Explode();
		}
	}
}
