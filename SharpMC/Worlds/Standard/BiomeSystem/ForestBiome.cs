using SharpMC.Worlds.Standard.Decorators;
using SharpMC.Worlds.Standard.Structures;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	internal class ForestBiome : BiomeBase
	{
		public override int Id
		{
			get { return 1; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 4; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator(), new ForestDecorator()}; }
		}

		public override Structure[] TreeStructures
		{
			get { return new Structure[] {new BirchTree(), new OakTree()}; }
		}

		public override float Temperature
		{
			get { return 0.7f; }
		}
	}
}