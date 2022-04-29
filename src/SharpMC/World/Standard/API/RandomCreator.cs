using SharpMC.Noise.API;

namespace SharpMC.World.Standard.API
{
    public delegate IGcRandom RandomCreator(int seed, (int X, int Z) pos);
}