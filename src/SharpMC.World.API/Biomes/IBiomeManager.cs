namespace SharpMC.World.API.Biomes
{
    public interface IBiomeManager
    {
        void AddBiomeType(IBiomeBase biome);

        IBiomeBase GetBiome(int x, int z);
    }
}