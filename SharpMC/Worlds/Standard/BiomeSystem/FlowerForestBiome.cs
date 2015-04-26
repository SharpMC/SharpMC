using SharpMC.Worlds.Standard.Decorators;
using SharpMC.Worlds.Standard.Structures;

namespace SharpMC.Worlds.Standard.BiomeSystem
{
	internal class FlowerForestBiome : BiomeBase
	{
		public override int Id
		{
			get { return 4; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 132; }
		}

		public override ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new GrassDecorator(), new TreeDecorator(), new FlowerDecorator()}; }
		}

		public override Structure[] TreeStructures
		{
			get { return new Structure[] {new BirchTree(), new OakTree()}; }
		}

		public override float Temperature
		{
			get { return 0.7f; }
		}

		/// <summary>
		///     Gets the maximum trees per chunk.
		/// </summary>
		/// <value>
		///     The maximum amount of trees per chunk.
		/// </value>
		public override int MaxTrees
		{
			get { return 30; }
		}

		public override int MinTrees
		{
			get { return 20; }
		}
	}
}