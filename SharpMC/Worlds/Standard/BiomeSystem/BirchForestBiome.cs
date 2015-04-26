using SharpMC.Worlds.Standard.Decorators;
using SharpMC.Worlds.Standard.Structures;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	internal class BirchForestBiome : BiomeBase
	{
		public override int Id
		{
			get { return 3; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 27; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator(), new ForestDecorator()}; }
		}

		public override Structure[] TreeStructures
		{
			get { return new Structure[] {new BirchTree()}; }
		}

		public override float Temperature
		{
			get { return 0.7f; }
		}
	}
}