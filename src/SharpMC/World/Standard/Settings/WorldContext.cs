using SharpMC.World.Noises;
using SharpMC.World.Standard.API;

namespace SharpMC.World.Standard.Settings
{
    internal class WorldContext : IWorldContext
    {
        public WorldContext(IWorldSettings settings, IRandomGenerator random)
        {
            Settings = settings;
            Random = random;
        }

        public IWorldSettings Settings { get; }
        public IRandomGenerator Random { get; }
    }
}