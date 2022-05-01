using Microsoft.Extensions.DependencyInjection;
using SharpMC.World.API;
using SharpMC.World.Nether;
using SharpMC.World.Standard;

namespace SharpMC.World
{
    public static class StandardExtensions
    {
        public static IServiceCollection AddNormalWorld(this IServiceCollection services)
        {
            return services
                .AddSingleton<IWorldProvider, StandardWorldProvider>()
                .AddSingleton<IWorldProvider, NetherWorldProvider>();
        }
    }
}