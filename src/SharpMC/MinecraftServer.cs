using Microsoft.Extensions.Logging;
using SharpMC.Logging;
using SharpMC.Net;
using SharpMC.Network;
using SharpMC.Network.Packets;
using SharpMC.Players;
using SharpMC.Util;
using SharpMC.World;
using System;
using System.IO;
using SharpMC.Admin;
using SharpMC.API.Enums;
using SharpMC.Network.API;
using SharpMC.Plugins;
using SharpMC.Plugins.Channel;

namespace SharpMC
{
    public class MinecraftServer : IServer
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(MinecraftServer));

        private bool _initiated;
        private NetServer Server { get; }
        public ServerInfo Info { get; }
        public EncryptionHolder RsaEncryption { get; }
        public IPlayerFactory PlayerFactory { get; set; }
        public LevelManager LevelManager { get; }

        public MinecraftServer(INetConfiguration netConfig)
        {
            Info = new ServerInfo(this);
            Log.LogInformation($"Initiating {ServerInfo.ProtocolName}");

            Log.LogInformation("Enabling global error handling...");
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledException;

            Log.LogInformation("Loading variables...");
            InitiateVariables();

            Log.LogInformation("Checking files and directories...");
            CheckDirectoriesAndFiles();

            Log.LogInformation("Loading plugins...");
            LoadPlugins();

            Log.LogInformation("Loading packets...");
            MCPacketFactory.Load();

            Server = new NetServer(netConfig, new McConnectionFactory(this));
            PlayerFactory = new DefaultPlayerFactory(this);
            LevelManager = new LevelManager();
            LevelManager.GetLevel(null, "default");

            Log.LogInformation("Generating RSA keypair...");
            RsaEncryption = new EncryptionHolder();
            _initiated = true;
        }

        private void CheckDirectoriesAndFiles()
        {
            if (Globals.Instance.LevelManager != null &&
                !Directory.Exists(Globals.Instance.LevelManager.MainLevel.LvlName))
                Directory.CreateDirectory(Globals.Instance.LevelManager.MainLevel.LvlName);
            if (!Directory.Exists("Players"))
                Directory.CreateDirectory("Players");
        }

        private void LoadPlugins()
        {
            Globals.Instance.PluginManager.LoadPlugins();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            Log.LogError(e, $"An unhandled exception occured! Error message: {e.Message}");
        }

        private void InitiateVariables()
        {
            var globals = Globals.Instance;

            globals.Rand = new Random();
            Console.Title = ServerInfo.ProtocolName;

            // TODO : globals.LevelManager = new LevelManager(LoadLevel());
            // TODO : globals.LevelManager.AddLevel("nether", new NetherLevel("nether"));
            // TODO : globals.ChatHandler = new Synchronized<ChatHandler>(new ChatHandler());
            // TODO : globals.ServerKey = PacketCryptography.GenerateKeyPair();
            // TODO : globals.ClientManager = new ClientManager();

            globals.ConsolePlayer = new Player(null, this, "Console")
            {
                Uuid = Guid.NewGuid(),
                Gamemode = GameMode.Spectator,
                Level = globals.LevelManager?.MainLevel
            };
            // TODO : globals.ConsolePlayer.Wrapper.Player = Globals.ConsolePlayer;
            globals.ConsolePlayer.IsOperator = true;

            globals.MessageFactory = new MessageFactory();
            globals.PluginManager = new PluginManager();
            // TODO : globals.ServerListener = new BasicListener();

            OperatorLoader.LoadOperators();
        }

        public void Start()
        {            
            if (!_initiated)
                throw new Exception("Server not initiated!");
            try
            {
                Log.LogInformation("Enabling plugins...");
                EnablePlugins();

                Server.Start();
                Log.LogInformation("Server ready for connections.");
            }
            catch (Exception ex)
            {
                UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        public void Stop()
        {
            Log.LogInformation("Disabling plugins...");
            DisablePlugins();

            Server.Stop();
            Log.LogInformation("Server stopped.");
        }

        private Level LoadLevel()
        {
            var lvlType = Config.Server.LevelType;
            Level lvl;
            switch (lvlType)
            {
                case LevelType.Flatland:
                    // TODO : lvl = new FlatLandLevel(Config.GetProperty("WorldName", "world"));
                    break;
                case LevelType.Standard:
                    // TODO : lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
                    break;
                case LevelType.Anvil:
                    // TODO : lvl = new AnvilLevel(Config.GetProperty("WorldName", "world"));
                    break;
                default:
                    // TODO : lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
                    break;
            }
            // TODO : return lvl;
            throw new InvalidOperationException();
        }

        private void EnablePlugins()
        {
            Globals.Instance.PluginManager.EnablePlugins(Globals.Instance.LevelManager);
        }

        private void DisablePlugins()
        {
            Globals.Instance.PluginManager.DisablePlugins();
        }
    }
}