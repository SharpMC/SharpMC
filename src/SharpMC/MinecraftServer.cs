using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpMC.API;
using SharpMC.API.Worlds;
using SharpMC.Config;
using SharpMC.Meta;
using SharpMC.Network.Packets;
using SharpMC.Plugin.API;

namespace SharpMC
{
    internal sealed class MinecraftServer : IServer
    {
        private readonly ILogger<MinecraftServer> _log;
        private readonly IOptions<ServerSettings> _settings;
        private readonly ILevelManager _levelManager;
        private readonly IPluginManager _pluginManager;

        public MinecraftServer(ILogger<MinecraftServer> log, IOptions<ServerSettings> cfg,
            ILevelManager levelManager, IPluginManager pluginManager)
        {
            _log = log;
            _settings = cfg;
            _levelManager = levelManager;
            _pluginManager = pluginManager;
        }

        private ServerInfo? Info { get; set; }

        public void Start()
        {
            _log.LogInformation("Enabling global error handling...");
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += HandleException;

            const string proto = ServerInfo.ProtocolName;
            _log.LogInformation($"Initiating {proto}");
            Info = new ServerInfo(_levelManager, _settings.Value);
            Console.Title = $"{nameof(SharpMC)} {proto}";

            _log.LogInformation("Loading plugins...");
            _pluginManager.LoadPlugins();

            _log.LogInformation("Enabling plugins...");
            _pluginManager.EnablePlugins(_levelManager);

            _log.LogInformation("Loading packets...");
            MCPacketFactory.Load();

            var comm = _settings.Value.Net!;
            _log.LogInformation("Listening on {net}...", comm.ToString());
            
            // Create server ?!
            // TODO Server.Start();
            _log.LogInformation("Server ready for connections.");
        }

        public void Stop()
        {
            _log.LogInformation("Disabling plugins...");
            _pluginManager.DisablePlugins();

            // TODO Server.Stop();
            Info = null;
            _log.LogInformation("Server stopped.");
        }

        private void HandleException(object _, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            var message = $"An unhandled exception occurred! Error message: {ex.Message}";
            _log.LogError(ex, message);
        }
    }
}