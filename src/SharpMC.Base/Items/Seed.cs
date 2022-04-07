using System.Numerics;
using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.World;

namespace SharpMC.Items
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