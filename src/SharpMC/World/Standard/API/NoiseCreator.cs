using SharpMC.Noise.API;

namespace SharpMC.World.Standard.API
{
    public delegate INoiseGenerator NoiseCreator(int seed, int octaves);
}