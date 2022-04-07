using SharpMC.Blocks;
using SharpMC.Core.Worlds.Standard.Decorators;
using SharpMC.Core.Worlds.Standard.Structures;

namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	public class BiomeBase : IBiome
	{
		public virtual double BaseHeight
		{
			get { return 52.0; }
		}

		public virtual int Id
		{
			get { return 0; }
		}

		public virtual byte MinecraftBiomeId
		{
			get { return 0; }
		}

		public virtual int MaxTrees
		{
			get { return 10; }
		}

		public virtual int MinTrees
		{
			get { return 0; }
		}

		public virtual Structure[] TreeStructures
		{
			get { return new Structure[] {new OakTree()}; }
		}

		public virtual ChunkDecorator[] Decorators
		{
			get { return new ChunkDecorator[] {new TreeDecorator()}; }
		}

		public virtual float Temperature
		{
			get { return 0.0f; }
		}

		public virtual Block TopBlock
		{
			get { return new BlockGrass(); }
		}

		public virtual Block Filling
		{
			get { return new Block(3); }
		}
	}
}