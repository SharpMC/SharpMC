using Microsoft.Extensions.DependencyInjection;
using SharpMC.World.API;
using SharpMC.World.API.Chunks;
using SharpMC.World.API.Storage;
using SharpMC.World.Common.Storage;

namespace SharpMC.World.Common
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            return services
                .AddSingleton<IChunkRepository, ChunkRepository>()
                .AddSingleton<ICompression, GZipCompression>()
                .AddSingleton<IWorldPackager, CommonPackager>();
        }
    }
}