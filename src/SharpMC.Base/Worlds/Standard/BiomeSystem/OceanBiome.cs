namespace SharpMC.Core.Worlds.Standard.BiomeSystem
{
	public class OceanBiome : BiomeBase
	{
		public override double BaseHeight
		{
			get { return 32; }
		}

		public override float Temperature
		{
			get { return 0.5f; }
		}

		public override byte MinecraftBiomeId
		{
			get { return 0; }
		}
	}
}