using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpMC.API;
using SharpMC.API.Players;
using SharpMC.API.Worlds;
using SharpMC.Config;
using SharpMC.Meta;
using SharpMC.Net;
using SharpMC.Network;
using SharpMC.Network.Packets;
using SharpMC.Players;
using SharpMC.Plugin.API;
using SharpMC.Util;

namespace SharpMC
{
    internal sealed class MinecraftServer : IServer
    {
        private readonly ILogger<MinecraftServer> _log;
        private readonly IOptions<ServerSettings> _settings;
        private readonly ILevelManager _levelManager;
        private readonly IPluginManager _pluginManager;
        private readonly ILoggerFactory _logFactory;

        public MinecraftServer(ILogger<MinecraftServer> log, IOptions<ServerSettings> cfg,
            ILevelManager levelManager, IPluginManager pluginManager, 
            ILoggerFactory logFactory)
        {
            _log = log;
            _settings = cfg;
            _levelManager = levelManager;
            _pluginManager = pluginManager;
            _logFactory = logFactory;
        }

        public IServerInfo? Info { get; set; }
        public IPlayerFactory PlayerFactory { get; set; }
        private NetServer? Server { get; set; }
        public IEncryption RsaEncryption { get; set; }

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

            _log.LogInformation("Preparing world...");
            var kind = _settings.Value.Level?.Type ?? default;
            _levelManager.GetLevel(kind);
            PlayerFactory = new PlayerFactory(this);

            _log.LogInformation("Generating RSA keypair...");
            var eLog = _logFactory.CreateLogger<EncryptionHolder>();
            RsaEncryption = new EncryptionHolder(eLog);

            var comm = _settings.Value.Net!;
            _log.LogInformation("Listening on {net}...", comm.ToString());

            var log = _logFactory.CreateLogger<NetServer>();
            var config = _settings.Value.Net!;
            var factory = new McConnectionFactory(this, _logFactory);
            Server = new NetServer(log, config, factory);
            Server.Start();
            _log.LogInformation("Server ready for connections.");
        }

        public void Stop()
        {
            _log.LogInformation("Disabling plugins...");
            _pluginManager.DisablePlugins();

            Server?.Stop();
            Server = null;
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