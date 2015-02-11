using SharpMCRewrite.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	public class Block
	{
		public INTVector3 Coordinates { get; set; }
		public ushort Id { get; set; }
		public bool IsReplacible { get; set; }
		public ushort Metadata { get; set; }
		public bool IsSolid { get; set; }
		public float Durability { get; set; }

		internal Block(ushort id)
		{
			Id = id;
			IsSolid = true;
			Durability = 0.5f;
		}

		public bool CanPlace(Level world)
		{
			return CanPlace(world, Coordinates);
		}

		protected virtual bool CanPlace(Level world, INTVector3 blockCoordinates)
		{
			return world.GetBlock(blockCoordinates).IsReplacible;
		}

		public virtual void BreakBlock(Level world)
		{
			world.SetBlock(new Block(0) { Coordinates = Coordinates });
		}

		public virtual bool PlaceBlock(Level world, Player player, INTVector3 blockCoordinates, BlockFace face)
		{
			// No default placement. Return unhandled.
			return false;
		}

		public virtual bool Interact(Level world, Player player, INTVector3 blockCoordinates, BlockFace face)
		{
			// No default interaction. Return unhandled.
			return false;
		}

		public static INTVector3 GetNewCoordinatesFromFace(INTVector3 target, BlockFace face)
		{
			INTVector3 intVector = new INTVector3((int) target.X, (int) target.Y, (int) target.Z);
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
			return Durability / 5.0F;
		}
	}
}
