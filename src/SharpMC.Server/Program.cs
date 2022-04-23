using System;
using System.Net;
using Microsoft.Extensions.Logging;
using SharpMC.API.Plugins;
using SharpMC.Logging;
using SharpMC.Network.API;
using SharpMC.Plugin.Admin;
using SharpMC.Plugin.Pets;
using SharpMC.Plugin.Test;
using SharpMC.Plugins;

namespace SharpMC.Server
{
    internal class Program
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Program));

        private static void Main()
        {
            Log.LogInformation("Initializing...");
            Config.Check();
            var config = Config.Server.Port;
            var comm = new NetConfiguration
            {
                Host = IPAddress.Any, Port = config, Protocol = ProtocolType.Tcp
            };
            Log.LogInformation("Listening on {0}...", comm.ToString());
            IServer server = new MinecraftServer(comm);
            try
            {
                server.Start();
                Console.ReadLine();
            }
            finally
            {
                server.Stop();
            }
            Log.LogInformation("Exited.");
        }

        // ReSharper disable UnusedMember.Local
        private static readonly IPlugin[] Dummy =
        {
            new MainPlugin(), new TestPlugin(), new PetPlugin()
        };
    }
}