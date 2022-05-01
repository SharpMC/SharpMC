using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpMC.API;
using SharpMC.Plugin.Admin;
using SharpMC.Plugin.API;
using SharpMC.World;
using SharpMC.World.Common;
using SharpMC.World.Flat;

namespace SharpMC.Server
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var root = AppContext.BaseDirectory;
            IHostEnv host = new CmdHost(root);
            return Host.CreateDefaultBuilder(args)
                .UseContentRoot(host.ContentRoot)
                .ConfigureServices((context, services)
                    => Configure(context, services.AddSingleton(host)));
        }

        private static void Configure(HostBuilderContext ctx, IServiceCollection services)
        {
            services
                .AddHostedService<Worker>()
                .AddServer(ctx.Configuration)
                .AddCommon()
                .AddFlatWorld()
                .AddNormalWorld()
                .AddSingleton<IPlugin, MainPlugin>();
        }
    }
}