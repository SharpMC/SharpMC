using System;
using Microsoft.Extensions.Logging;
using SharpCore;
using SharpMC.Log;
using TestPlugin;
using L = Microsoft.Extensions.Logging.LoggerFactory;

namespace SharpMC
{
    internal static class Program
    {
        private static MinecraftServer _server;

        private static void Main(string[] args)
        {
            // ReSharper disable ObjectCreationAsStatement
            new Main();
            new Test();
            // ReSharper restore ObjectCreationAsStatement

            using var logFactory = L.Create(builder => builder.AddConsole());
            LogManager.Factory = logFactory;

            _server = new MinecraftServer();
            _server.Start();
            Console.ReadLine();
            _server.Stop();
        }
    }
}