using SharpMC.Classes;
using SharpMC.Enums;
using SharpMC.Items;

namespace SharpMC.Blocks
{
	public class Block : Item
	{
		internal Block(ushort id) : base(id, 0)
		{
			Id = id;
			IsSolid = true;
			Durability = 0.5f;
			Metadata = 0;
			Drops = this;
		}

		public Vector3 Coordinates { get; set; }
		public bool IsReplacible { get; set; }
		public bool IsSolid { get; set; }
		public float Durability { get; set; }
		public Block Drops { get; set; }

		public bool CanPlace(Level world)
		{
			return CanPlace(world, Coordinates);
		}

		protected virtual bool CanPlace(Level world, Vector3 blockCoordinates)
		{
			return world.GetBlock(blockCoordinates).IsReplacible;
		}

		public virtual void BreakBlock(Level world)
		{
			world.SetBlock(new Block(0) {Coordinates = Coordinates});
		}

		public virtual bool PlaceBlock(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			// No default placement. Return unhandled.
			return false;
		}

		public virtual bool Interact(Level world, Player player, Vector3 blockCoordinates, BlockFace face)
		{
			// No default interaction. Return unhandled.
			return false;
		}

		public static Vector3 GetNewCoordinatesFromFace(Vector3 target, BlockFace face)
		{
			var intVector = new Vector3(target.X, target.Y, target.Z);
			switch (face)
			{
				case BlockFace.NegativeY:
					intVector.Y--;
					break;
				case BlockFace.PositiveY:
					intVector.Y++;
					break;
				case BlockFace.NegativeZ:
					intVector.Z--;
					break;
				case BlockFace.PositiveZ:
					intVector.Z++;
					break;
				case BlockFace.NegativeX:
					intVector.X--;
					break;
				case BlockFace.PositiveX:
					intVector.X++;
					break;
				default:
					break;
			}
			return intVector;
		}

		public float GetHardness()
		{
			return Durability/5.0F;
		}
	}
}