using SharpMC.Worlds.Standard.Decorators;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	internal class SunFlowerPlainsBiome : BiomeBase
	{
		public override int Id
		{
			get { return 5; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 129; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator(), new SunFlowerDecorator()}; }
		}

		public override float Temperature
		{
			get { return 0.8f; }
		}

		public override int MaxTrees
		{
			get { return 0; }
		}
	}
}