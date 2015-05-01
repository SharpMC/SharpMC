using SharpMC.Blocks;
using SharpMC.Utils;

namespace SharpMC.Worlds.Standard.Structures
{
	internal class BirchTree : Structure
	{
		private readonly int LeafRadius = 2;

		public override string Name
		{
			get { return "BirchTree"; }
		}

		public override int Height
		{
			get { return 10; }
		}

		//Enable to use vanilla tree's
		/*	public override void Create(ChunkColumn chunk, int x, int y, int z)
		{
			var location = new Vector3(x, y, z);
			if (!ValidLocation(new Vector3(x, y, z))) return;

			Random R = new Random();
			int Height = R.Next(4, 5);
			GenerateColumn(chunk, location, Height, new Block(17) {Metadata = 2});
			Vector3 LeafLocation = location + new Vector3(0, Height, 0);
			GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, new Block(18) {Metadata = 2});

		}*/

		public override Block[] Blocks
		{
			get
			{
				return new[]
				{
					new Block(17) {Coordinates = new Vector3(0, 0, 0), Metadata = 2},
					new Block(17) {Coordinates = new Vector3(0, 1, 0), Metadata = 2},
					new Block(17) {Coordinates = new Vector3(0, 2, 0), Metadata = 2},
					new Block(17) {Coordinates = new Vector3(0, 3, 0), Metadata = 2},
					new Block(17) {Coordinates = new Vector3(0, 4, 0), Metadata = 2},
					new Block(17) {Coordinates = new Vector3(0, 5, 0), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(-1, 4, 1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(1, 4, -1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(-1, 4, -1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(1, 4, 1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(-1, 4, 0), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(1, 4, 0), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(0, 4, -1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(0, 4, 1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(-1, 5, 0), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(1, 5, 0), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(0, 5, -1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(0, 5, 1), Metadata = 2},
					new Block(18) {Coordinates = new Vector3(0, 6, 0), Metadata = 2}
				};
			}
		}

		public bool ValidLocation(Vector3 location)
		{
			if (location.X - LeafRadius < 0 || location.X + LeafRadius >= 16 || location.Z - LeafRadius < 0 ||
			    location.Z + LeafRadius >= 256)
				return false;
			return true;
		}
	}
}