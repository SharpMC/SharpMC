using SharpMC.Blocks;
using SharpMC.Worlds.Standard.BiomeSystem;

namespace SharpMC.Worlds.Standard.Decorators
{
	public class OreDecorator : ChunkDecorator
	{
		private ChunkColumn chunke;

		public override void Decorate(ChunkColumn chunk, BiomeBase biome, int x, int z)
		{
			chunke = chunk;
			for (var y = 1 /*No need to check first layer :p*/; y < 128 /*Nothing above this*/; y++)
			{
				if (chunk.GetBlock(x, y, z) == 1)
				{
					if (y < 128)
					{
						GenerateCoal(x, y, z);
					}

					if (y < 64)
					{
						GenerateIron(x, y, z);
					}

					if (y < 29)
					{
						GenerateGold(x, y, z);
					}

					if (y < 23)
					{
						GenerateLapis(x, y, z);
					}

					if (y < 16)
					{
						if (y > 12)
						{
							if (StandardWorldProvider.GetRandomNumber(0, 3) == 2)
							{
								GenerateDiamond(x, y, z);
							}
						}
						else
						{
							GenerateDiamond(x, y, z);
						}
					}
				}
			}
		}

		public void GenerateCoal(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 35) == 1)
			{
				chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(16));
			}
		}

		public void GenerateIron(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 65) == 1)
			{
				chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(15));
			}
		}

		public void GenerateGold(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 80) == 1)
			{
				chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(14));
			}
		}

		public void GenerateDiamond(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 130) == 1)
			{
				chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(56));
			}
		}

		public void GenerateLapis(int x, int y, int z)
		{
			if (StandardWorldProvider.GetRandomNumber(0, 80) == 1)
			{
				chunke.SetBlock(x, y, z, BlockFactory.GetBlockById(21));
			}
		}
	}
}