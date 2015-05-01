using SharpMC.Blocks;
using SharpMC.Enums;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Items
{
	internal class ItemFlintAndSteel : Item
	{
		public ItemFlintAndSteel(byte metadata) : base(259, metadata)
		{
		}

		public override void UseItem(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			ConsoleFunctions.WriteDebugLine("USED FLINT AND STEEL <3");
			var block = world.GetBlock(blockCoordinates);
			if (block.Id != 46)
			{
				var affectedBlock = world.GetBlock(GetNewCoordinatesFromFace(blockCoordinates, BlockFace.PositiveY));
				if (affectedBlock.Id == 0)
				{
					var fire = new BlockFire
					{
						Coordinates = affectedBlock.Coordinates
					};
					world.SetBlock(fire);
				}
			}
			else
			{
				world.SetBlock(new BlockAir {Coordinates = block.Coordinates});
				//new PrimedTnt(world)
				//{
				//	KnownPosition = new PlayerLocation(blockCoordinates.X, blockCoordinates.Y, blockCoordinates.Z),
				//	Fuse = (byte)(new Random().Next(0, 20) + 10)
				//}.SpawnEntity();
			}
		}
	}
}