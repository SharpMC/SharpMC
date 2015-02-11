using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	class BlockFire : Block
	{
		internal BlockFire() : base(51)
		{
			IsReplacible = true;
			IsSolid = false;
		}
	}
}
