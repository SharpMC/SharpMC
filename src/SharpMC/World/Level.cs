using System;
using Microsoft.Extensions.Logging;
using SharpMC.Logging;
using SharpMC.Players;
using SharpMC.World.Generators;

namespace SharpMC.World
{
    public class Level : IDisposable
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(Level));

        public int PlayerCount { get; private set; }
        public string Name { get; set; }

        public Level(string name, FlatWorldGenerator generator)
        {
            Name = name;
            PlayerCount = 0;
        }

        public void Dispose()
        {
        }

        public void RemovePlayer(Player player)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            // TODO: throw new NotImplementedException();
        }
    }
}