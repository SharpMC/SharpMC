using SharpMC.Blocks;
using SharpMC.Utils;

namespace SharpMC.Worlds.Standard.Structures
{
	//Hehe, not finished yet :P
	internal class Monument : Structure
	{
		public override string Name
		{
			get { return "Monument"; }
		}

		public override Block[] Blocks
		{
			get
			{
				return new[]
				{
					new Block(42) {Metadata = 1, Coordinates = new Vector3(0, 0, 0)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(0, 0, 0)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(1, 0, 0)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(2, 0, 0)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(3, 0, 0)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(0, 0, 1)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(1, 0, 1)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(2, 0, 1)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(3, 0, 1)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(0, 0, 2)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(1, 0, 2)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(2, 0, 2)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(3, 0, 2)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(0, 0, 3)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(1, 0, 3)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(2, 0, 3)},
					new Block(43) {Metadata = 1, Coordinates = new Vector3(3, 0, 3)}
				};
			}
		}
	}
}