using SharpMC.World.Noises;
using SharpMC.World.Standard.Settings;

namespace SharpMC.World.Standard.API
{
    public interface IWorldContext
    {
        IWorldSettings Settings { get; }

        IRandomGenerator Random { get; }
    }
}