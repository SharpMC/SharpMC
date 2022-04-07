using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.World;
using SharpMC.Enums;
using SharpMC.Items;
using SharpMC.Util;
using PlayerLocation = SharpMC.Util.PlayerLocation;
using Vector3 = System.Numerics.Vector3;

namespace SharpMC.Blocks
{
	public class Block : Item
	{
        public ushort Id { get; }
        public byte Metadata { get; internal set; }

        public virtual void OnPlace()
        {

        }

		internal Block(ushort id, byte metadata = 0) : base(id, metadata)
		{
			Id = id;
			Durability = 0.5f;
			Metadata = metadata;
			Drops = new[] {new ItemStack(this, 1)};
			

			IsSolid = true;
			IsBuildable = true;
			IsReplacible = false;
			IsTransparent = false;

			FuelEfficiency = 0;
			IsBlock = true;
		}

        public BlockCoordinates Coordinates2 { get; set; }
        public Vector3 Coordinates { get; set; }
		public bool IsReplacible { get; set; }
        public bool IsSolid { get; protected set; }
        public bool IsTransparent { get; set; }
		public float Durability { get; set; }
		public ItemStack[] Drops { get; set; }
		public bool IsBuildable { get; set; }

		public bool CanPlace(Level world)
		{
			return CanPlace(world, Coordinates);
		}

		public void DoDrop(Level world)
		{
			if (Drops == null) return;
			foreach (var its in Drops)
			{
				new ItemEntity(world, its)
				{
					KnownPosition = new PlayerLocation(Coordinates.X, Coordinates.Y + 0.25, Coordinates.Z)
				}.SpawnEntity();
			}
		}

		protected virtual bool CanPlace(Level world, Vector3 blockCoordinates)
		{
			return world.GetBlock(blockCoordinates).IsReplacible;
		}

		public virtual void BreakBlock(Level world)
		{
			world.SetBlock(new Block(0) {Coordinates = Coordinates});
		}

		public virtual bool PlaceBlock(Level world, Player player, Vector3 blockCoordinates, BlockFace face, Vector3 mouseLocation)
		{
			// No default placement. Return unhandled.
			return false;
		}

		public float GetHardness()
		{
			return Durability/5.0F;
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
				new Vector3(Coordinates.X, Coordinates.Y, Coordinates.Z),
				new Vector3(Coordinates.X + 1, Coordinates.Y + 1, Coordinates.Z + 1));
		}
	}
}