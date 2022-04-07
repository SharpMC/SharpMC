using SharpMC.Core.Worlds.Standard.Decorators;

namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	internal class PlainsBiome : BiomeBase
	{
		public override int Id
		{
			get { return 0; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 1; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator()}; }
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