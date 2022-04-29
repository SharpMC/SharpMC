using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SharpMC.Admin;
using SharpMC.Logging;
using SharpMC.Noise.API;
using SharpMC.Noise.Lib;
using SharpMC.Players;
using SharpMC.Storage;
using SharpMC.Storage.API;
using SharpMC.World.Anvil;
using SharpMC.World.Common;
using SharpMC.World.Generators;
using SharpMC.World.Nether;
using SharpMC.World.Standard;

namespace SharpMC.World
{
    public class LevelManager
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(LevelManager));

        private ConcurrentDictionary<string, Level> Levels { get; }

        public LevelManager()
        {
            Levels = new ConcurrentDictionary<string, Level>();
        }

        public virtual Level GetLevel(Player player, string name)
        {
            return Levels.GetOrAdd(name, CreateLevel);
        }

        private Level CreateLevel(string name)
        {
            Enum.TryParse<LevelType>(name, true, out var type);
            Log.LogInformation($"Creating level '{name}' as '{type}'...");
            var provider = LoadLevel(type);
            IWorldGenerator generator;
            if (provider is IWorldGenerator gen)
                generator = gen;
            else
                generator = Wrap(provider);
            var level = new Level(name, generator);
            level.Initialize();
            return level;
        }

        private IWorldProvider LoadLevel(LevelType lvlType)
        {
            INoiseGenerator Creator(int seed, int octaves)
                => new SimplexOctaveGenerator(seed, octaves);

            IGcRandom RandomGen(int seed, (int X, int Z) pos)
                => new GcRandom(seed, pos);

            switch (lvlType)
            {
                case LevelType.Standard:
                    return new StandardWorldProvider(Creator, RandomGen);
                case LevelType.Flatland:
                    return new FlatWorldGenerator();
                case LevelType.Anvil:
                    return new AnvilWorldProvider();
                case LevelType.Nether:
                    return new NetherWorldProvider(Creator);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lvlType), lvlType, null);
            }
        }

        private IWorldGenerator Wrap(IWorldProvider parent)
        {
            var pType = parent.GetType().Name;
            var folder = pType.Replace("WorldProvider", string.Empty);
            folder = Path.Combine(nameof(World), folder);
            ICompression compression = new GZipCompression();
            IChunkRepository repo = new ChunkRepository(folder, compression);
            IWorldGenerator cache = new CacheWorldProvider(parent, repo);
            return cache;
        }

        public virtual void RemoveLevel(Level level)
        {
            if (Levels.TryRemove(level.Name, out _))
            {
                level.Dispose();
            }
        }

        public Level[] GetLevels()
        {
            return Levels.Values.ToArray();
        }

        public Level MainLevel { get; set; }

        public Player[] GetAllPlayers()
        {
            // TODO ?!
            return Array.Empty<Player>();
        }
    }
}