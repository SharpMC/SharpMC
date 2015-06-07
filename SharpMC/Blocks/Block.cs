#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Blocks
{
	using SharpMC.Entity;
	using SharpMC.Enums;
	using SharpMC.Items;
	using SharpMC.Utils;
	using SharpMC.Worlds;

	public class Block : Item
	{
		internal Block(ushort id)
			: base(id, 0)
		{
			this.Id = id;
			this.Durability = 0.5f;
			this.Metadata = 0;
			this.Drops = new[] { new ItemStack(this, 1) };
			

			this.IsSolid = true;
			this.IsBuildable = true;
			this.IsReplacible = false;
			this.IsTransparent = false;

			this.FuelEfficiency = 0;
		}

		public Vector3 Coordinates { get; set; }

		public bool IsReplacible { get; set; }

		public bool IsSolid { get; set; }

		public bool IsTransparent { get; set; }

		public float Durability { get; set; }

		public ItemStack[] Drops { get; set; }

		public bool IsBuildable { get; set; }

		public bool CanPlace(Level world)
		{
			return this.CanPlace(world, this.Coordinates);
		}

		protected virtual bool CanPlace(Level world, Vector3 blockCoordinates)
		{
			return world.GetBlock(blockCoordinates).IsReplacible;
		}

		public virtual void BreakBlock(Level world)
		{
			world.SetBlock(new Block(0) { Coordinates = this.Coordinates });
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

		public float GetHardness()
		{
			return this.Durability / 5.0F;
		}

		public virtual void OnTick(Level level)
		{
		}

		public virtual void DoPhysics(Level level)
		{
		}

		public BoundingBox GetBoundingBox()
		{
			return new BoundingBox(
				new Vector3(this.Coordinates.X, this.Coordinates.Y, this.Coordinates.Z), 
				new Vector3(this.Coordinates.X + 1, this.Coordinates.Y + 1, this.Coordinates.Z + 1));
		}
	}
}