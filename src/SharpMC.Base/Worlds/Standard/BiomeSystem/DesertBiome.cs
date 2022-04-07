using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.Structures;

namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	internal class DesertBiome : BiomeBase
	{
		public override int Id
		{
			get { return 2; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 2; }
		}

		public override float Temperature
		{
			get { return 2.0f; }
		}

		public override Block TopBlock
		{
			get { return new Block(12); }
		}

		public override Block Filling
		{
			get { return new BlockSandStone(); }
		}

		public override Structure[] TreeStructures
		{
			get { return new Structure[] {new CactusStructure()}; }
		}

		public override int MaxTrees
		{
			get { return 4; }
		}
	}
}