using System;
using SharpMC.Blocks;
using SharpMC.Utils;

namespace SharpMC.Worlds.Standard.Structures
{
	internal class PineTree : Structure
	{
		private readonly int LeafRadius = 2;
		private int BottomSpace = 2;

		public override string Name
		{
			get { return "PineTree"; }
		}

		public override int Height
		{
			get { return 10; }
		}

		public bool ValidLocation(Vector3 location)
		{
			if (location.X - LeafRadius < 0 || location.X + LeafRadius >= 16 || location.Z - LeafRadius < 0 ||
			    location.Z + LeafRadius >= 256)
				return false;
			return true;
		}

		public override void Create(ChunkColumn chunk, int x, int y, int z)
		{
			var location = new Vector3(x, y, z);
			if (!ValidLocation(new Vector3(x, y, z))) return;

			var R = new Random();
			var Height = R.Next(7, 8);
			GenerateColumn(chunk, location, Height, new Block(17) {Metadata = 1});
			for (var Y = 1; Y < Height; Y++)
			{
				if (Y%2 == 0)
				{
					GenerateVanillaCircle(chunk, location + new Vector3(0, Y + 1, 0), LeafRadius - 1, new Block(18) {Metadata = 1});
					continue;
				}
				GenerateVanillaCircle(chunk, location + new Vector3(0, Y + 1, 0), LeafRadius, new Block(18) {Metadata = 1});
			}

			GenerateTopper(chunk, location + new Vector3(0, Height, 0), 0x1);
		}

		protected void GenerateTopper(ChunkColumn chunk, Vector3 location, byte type = 0x0)
		{
			var SectionRadius = 1;
			GenerateCircle(chunk, location, SectionRadius, new Block(18) {Metadata = 1});
			var top = location + new Vector3(0, 1, 0);
			var x = (int) location.X;
			var y = (int) location.Y + 1;
			var z = (int) location.Z;
			chunk.SetBlock(x, y, z, new Block(18) {Metadata = 1});
			if (type == 0x1 && y < 256)
				GenerateVanillaCircle(chunk, new Vector3(x, y, z), SectionRadius, new Block(18) {Metadata = 1});
		}
	}
}