namespace SharpMCRewrite.Worlds.ExperimentalV2
{
	internal class CaveGenerator
	{
		private readonly SimplexOctaveGenerator caveNoise;

		public CaveGenerator(int seed)
		{
			caveNoise = new SimplexOctaveGenerator(seed + 22, 3);
			caveNoise.SetScale(1.0/100);
		}

		public void GenerateCave(ChunkColumn chunk, int caveX, int caveY, int caveZ)
		{
			//Yes, we did have caves. Removed because of buggyness
		}
	}
}