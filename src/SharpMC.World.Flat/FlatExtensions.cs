using Microsoft.Extensions.DependencyInjection;
using SharpMC.World.API;

namespace SharpMC.World.Flat
{
    public static class FlatExtensions
    {
        public static IServiceCollection AddFlatWorld(this IServiceCollection services)
        {
            return services
                .AddSingleton<IWorldProvider, FlatWorldGenerator>();
        }
    }
}