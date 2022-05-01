using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpMC.API;
using SharpMC.API.Entities;
using SharpMC.API.Players;
using SharpMC.API.Worlds;
using SharpMC.Config;
using SharpMC.Entities;
using SharpMC.Players;
using SharpMC.Plugin.API;
using SharpMC.Plugins;
using SharpMC.World;
using SharpMC.World.API.Chunks;

namespace SharpMC
{
    public static class ServerExtensions
    {
        public static IServiceCollection AddServer(this IServiceCollection services,
            IConfiguration config, string key = "Server")
        {
            var section = config.GetSection(key);
            services.AddOptions<ServerSettings>().Bind(section);

            return services
                .AddSingleton<IServer, MinecraftServer>()
                .AddSingleton<ILevelManager, LevelManager>()
                .AddSingleton<IPluginManager, PluginManager>()
                .AddSingleton<IPermissionManager, PermissionManager>()
                .AddSingleton<IPlayerFactory, PlayerFactory>()
                .AddSingleton<IEntityManager, EntityManager>()
                .AddSingleton<IChunkColumnFactory, ChunkColumnFactory>();
        }
    }
}