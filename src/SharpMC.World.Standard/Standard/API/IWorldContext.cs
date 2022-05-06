using SharpMC.World.API.Noises;

namespace SharpMC.World.Standard.API
{
    public interface IWorldContext
    {
        IWorldSettings Settings { get; }

        IRandomGenerator Random { get; }
    }
}