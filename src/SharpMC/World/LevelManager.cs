using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SharpMC.API;
using SharpMC.API.Entities;
using SharpMC.API.Worlds;
using SharpMC.World.API;

namespace SharpMC.World
{
    internal sealed class LevelManager : ILevelManager, IDisposable
    {
        private readonly ILogger<LevelManager> _log;
        private readonly IList<IWorldProvider> _providers;
        private readonly IEntityManager _entityManager;
        private readonly ILoggerFactory _factory;
        private readonly IWorldPackager _packager;

        private ConcurrentDictionary<LevelType, Level> Levels { get; }

        public LevelManager(ILogger<LevelManager> log, IEnumerable<IWorldProvider> providers, 
            IEntityManager entityManager, ILoggerFactory factory, IWorldPackager packager)
        {
            _log = log;
            _entityManager = entityManager;
            _factory = factory;
            _packager = packager;
            Levels = new ConcurrentDictionary<LevelType, Level>();
            _providers = providers.ToList();
        }

        public ILevel? MainLevel => Levels.Values.FirstOrDefault();

        public ILevel[] GetLevels() => Levels.Values.Cast<ILevel>().ToArray();

        public void TeleportToMain(IPlayer player) 
            => TeleportToLevel(player, MainLevel?.GetType().ToString() ?? "?");

        public void TeleportToLevel(IPlayer player, string levelName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPlayer> GetAllPlayers() 
            => GetLevels().SelectMany(l => l.GetPlayers());

        public ILevel GetLevel(LevelType kind) 
            => Levels.GetOrAdd(kind, CreateLevel);

        public void RemoveLevel(LevelType kind)
        {
            if (Levels.TryRemove(kind, out var level))
            {
                level.Dispose();
            }
        }

        private Level CreateLevel(LevelType kind)
        {
            _log.LogInformation($"Creating level '{kind}'...");
            var provider = _providers.FirstOrDefault(p => IsMatch(p, kind));
            if (provider == null)
            {
                throw new InvalidOperationException(kind.ToString());
            }
            var generator = _packager.Wrap(provider);
            var log = _factory.CreateLogger<Level>();
            var level = new Level(log, generator, _entityManager);
            level.Initialize();
            return level;
        }

        private static bool IsMatch(IWorldProvider provider, LevelType kind)
        {
            var prefix = kind.ToString();
            var name = provider.GetType().Name;
            return name.StartsWith(prefix) ||
                   name.StartsWith(prefix.Replace("land", ""));
        }

        public void Dispose()
        {
            Levels.Clear();
        }
    }
}