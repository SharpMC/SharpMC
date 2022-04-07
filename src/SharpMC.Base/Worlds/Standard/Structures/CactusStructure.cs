using System.Numerics;
using SharpMC.Blocks;

namespace SharpMC.Core.Worlds.Standard.Structures
{
	internal class CactusStructure : Structure
	{
		public override string Name
		{
			get { return "Cactus"; }
		}

		public override int Height
		{
			get { return 3; }
		}

		public override Block[] Blocks
		{
			get
			{
				return new[]
				{
					new Block(81) {Coordinates = new Vector3(0, 0, 0)},
					new Block(81) {Coordinates = new Vector3(0, 1, 0)},
					new Block(81) {Coordinates = new Vector3(0, 2, 0)}
				};
			}
		}
	}
}