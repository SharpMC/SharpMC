using System.Numerics;
using SharpMC.Blocks;

namespace SharpMC.Core.Worlds.Standard.Structures
{
	internal class OakTree : Structure
	{
		private readonly int _leafRadius = 2;

		public override string Name
		{
			get { return "OakTree"; }
		}

		public override int Height
		{
			get { return 10; }
		}

		//Enable to use vanilla tree's
		/*
		public override void Create(ChunkColumn chunk, int x, int y, int z)
		{
			var location = new Vector3(x, y, z);
			if (!ValidLocation(new Vector3(x, y, z))) return;

			Random R = new Random();
			int Height = R.Next(4, 5);
			GenerateColumn(chunk, location, Height, new Block(17));
			Vector3 LeafLocation = location + new Vector3(0, Height, 0);
			GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, new Block(18));
			
		}*/

		public override Block[] Blocks
		{
			get
			{
				return new[]
				{
					new Block(17) {Coordinates = new Vector3(0, 0, 0)},
					new Block(17) {Coordinates = new Vector3(0, 1, 0)},
					new Block(17) {Coordinates = new Vector3(0, 2, 0)},
					new Block(17) {Coordinates = new Vector3(0, 3, 0)},
					new Block(17) {Coordinates = new Vector3(0, 4, 0)},
					new Block(17) {Coordinates = new Vector3(0, 5, 0)},
					new Block(18) {Coordinates = new Vector3(-1, 4, 1)},
					new Block(18) {Coordinates = new Vector3(1, 4, -1)},
					new Block(18) {Coordinates = new Vector3(-1, 4, -1)},
					new Block(18) {Coordinates = new Vector3(1, 4, 1)},
					new Block(18) {Coordinates = new Vector3(-1, 4, 0)},
					new Block(18) {Coordinates = new Vector3(1, 4, 0)},
					new Block(18) {Coordinates = new Vector3(0, 4, -1)},
					new Block(18) {Coordinates = new Vector3(0, 4, 1)},
					new Block(18) {Coordinates = new Vector3(-1, 5, 0)},
					new Block(18) {Coordinates = new Vector3(1, 5, 0)},
					new Block(18) {Coordinates = new Vector3(0, 5, -1)},
					new Block(18) {Coordinates = new Vector3(0, 5, 1)},
					new Block(18) {Coordinates = new Vector3(0, 6, 0)}
				};
			}
		}

		public bool ValidLocation(Vector3 location)
		{
			if (location.X - _leafRadius < 0 || location.X + _leafRadius >= 16 || location.Z - _leafRadius < 0 ||
			    location.Z + _leafRadius >= 256)
				return false;
			return true;
		}
	}
}