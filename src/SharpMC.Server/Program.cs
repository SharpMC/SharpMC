using System;
using Microsoft.Extensions.Logging;
using SharpMC.API.Plugins;
using SharpMC.Logging;
using SharpMC.Plugin.Admin;
using SharpMC.Plugin.Pets;
using SharpMC.Plugin.Test;

namespace SharpMC.Server
{
    internal class Program
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            Log.LogInformation("Initializing...");
            IServer server = new MinecraftServer();
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