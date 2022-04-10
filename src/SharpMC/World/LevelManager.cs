using System.Collections.Concurrent;
using System.Linq;
using SharpMC.Players;
using SharpMC.World.Generators;

namespace SharpMC.World
{
    public class LevelManager
    {
        private ConcurrentDictionary<string, Level> Levels { get; }
            = new ConcurrentDictionary<string, Level>();

        public virtual Level GetLevel(Player player, string name)
        {
            return Levels.GetOrAdd(name, CreateLevel);
        }

        private Level CreateLevel(string name)
        {
            var level = new Level(name, new FlatWorldGenerator());
            level.Initialize();
            return level;
        }

        public virtual void RemoveLevel(Level level)
        {
            if (Levels.TryRemove(level.Name, out var oldLevel))
            {
                level.Dispose();
            }
        }

        public Level[] GetLevels()
        {
            return Levels.Values.ToArray();
        }
    }
}