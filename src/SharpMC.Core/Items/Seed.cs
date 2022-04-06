using SharpMC.Core.Blocks;
using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Items
{
	public class Seed : Item
	{
		public Block BecomesBlock { get; private set; }
		internal Seed(ushort id, byte metadata) : base(id, metadata)
		{
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			
		}
	}
}
