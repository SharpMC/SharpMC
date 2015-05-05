using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Entity;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC
{
	public class LevelManager
	{
		private Dictionary<string, Level> SubLevels { get; set; } 
		public Level MainLevel { get; private set; }
		public LevelManager(Level mainLevel)
		{
			MainLevel = mainLevel;
			SubLevels = new Dictionary<string, Level>();
		}

		public Level[] GetLevels()
		{
			return SubLevels.Values.ToArray();
		}

		public void AddLevel(string name, Level lvl)
		{
			ConsoleFunctions.WriteInfoLine("Initiating level: " + name);
			SubLevels.Add(name, lvl);
		}

		private Level GetLevel(string name)
		{
			var d = (from lvl in SubLevels where lvl.Key == name select lvl.Value).FirstOrDefault();
			if (d != null) return d;
			else return MainLevel;
		}

		public void TeleportToLevel(Player player, string level)
		{
			var lvl = GetLevel(level);
			
			player.Level.RemovePlayer(player);
			player.Level.BroadcastPlayerRemoval(player.Wrapper);

			player.Level = lvl;

			new Respawn(player.Wrapper) { Dimension = 0, Difficulty = (byte) lvl.Difficulty, GameMode = (byte) lvl.DefaultGamemode }.Write();
			
			player.IsSpawned = false;
			player.KnownPosition = lvl.GetSpawnPoint();
			player.SendChunksForKnownPosition(true);
		}

		public void TeleportToMain(Player player)
		{
			player.Level.RemovePlayer(player);
			player.Level.BroadcastPlayerRemoval(player.Wrapper);

			player.Level = MainLevel;

			new Respawn(player.Wrapper) { Dimension = 0, Difficulty = (byte)MainLevel.Difficulty, GameMode = (byte)MainLevel.DefaultGamemode }.Write();

			player.IsSpawned = false;
			player.KnownPosition = MainLevel.GetSpawnPoint();
			player.SendChunksForKnownPosition(true);
		}
	}
}
