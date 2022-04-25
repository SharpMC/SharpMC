using SharpMC.World.Standard.API;

namespace SharpMC.World.API
{
    public interface IBiomeManager
    {
        void AddBiomeType(IBiomeBase biome);

        IBiomeBase GetBiome(int x, int z);
    }
}