using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SharpMC.Core;
using SharpMC.Core.Networking.Packages;
using SharpMC.World.Generators;

namespace SharpMC.World
{
	public class LevelManager
	{
		private ConcurrentDictionary<string, Level> Levels { get; }  = new ConcurrentDictionary<string, Level>();

		public virtual Level GetLevel(Player player, string name)
		{
			return Levels.GetOrAdd(name, CreateLevel);
		}

		private Level CreateLevel(string s)
		{
			Level level = new Level(s, new FlatWorldGenerator());
			level.Initialize();
			return level;
		}

		public virtual void RemoveLevel(Level level)
		{
			Level oldLevel;
			if (Levels.TryRemove(level.Name, out oldLevel))
			{
				level.Dispose();
			}
		}

		public Level[] GetLevels()
		{
			return Levels.Values.ToArray();
		}

        public LevelManager()
        {
        }

		public LevelManager(Level mainLevel)
		{
			MainLevel = mainLevel;
			SubLevels = new Dictionary<string, Level>();
		}

		private Dictionary<string, Level> SubLevels { get; set; }
		public Level MainLevel { get; private set; }

		public void AddLevel(string name, Level lvl)
		{
			ConsoleFunctions.WriteInfoLine("Initiating level: " + name);
			SubLevels.Add(name, lvl);
		}

		private Level GetLevel(string name)
		{
			var d = (from lvl in SubLevels where lvl.Key == name select lvl.Value).FirstOrDefault();
			if (d != null) return d;
			return MainLevel;
		}

		public void TeleportToLevel(Player player, string level)
		{
			var lvl = GetLevel(level);

			player.Level.RemovePlayer(player);
			player.Level.BroadcastPlayerRemoval(player.Wrapper);

			player.Level = lvl;

			new Respawn(player.Wrapper)
			{
				Dimension = lvl.Dimension,
				Difficulty = (byte) lvl.Difficulty,
				GameMode = (byte) lvl.DefaultGamemode
			}.Write();

			player.IsSpawned = false;
			player.KnownPosition = lvl.GetSpawnPoint();
			player.SendChunksForKnownPosition(true);
		}

		public void TeleportToMain(Player player)
		{
			player.Level.RemovePlayer(player);
			player.Level.BroadcastPlayerRemoval(player.Wrapper);

			player.Level = MainLevel;

			new Respawn(player.Wrapper)
			{
				Dimension = 0,
				Difficulty = (byte) MainLevel.Difficulty,
				GameMode = (byte) MainLevel.DefaultGamemode
			}.Write();

			player.IsSpawned = false;
			player.KnownPosition = MainLevel.GetSpawnPoint();
			player.SendChunksForKnownPosition(true);
		}

		public void SaveAllChunks()
		{
			foreach (Level lvl in GetLevels())
			{
				lvl.SaveChunks();
			}
			MainLevel.SaveChunks();
		}

		public Player[] GetAllPlayers()
		{
			List<Player> players = new List<Player>();
			foreach (Level lvl in GetLevels())
			{
				players.AddRange(lvl.GetOnlinePlayers);
			}
			players.AddRange(MainLevel.GetOnlinePlayers);
			return players.ToArray();
		}
	}
}