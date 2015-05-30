using System;
using SharpMC.API;
using SharpMC.Entity;

namespace TestPlugin
{
	[Plugin]
    public class TestPlugin : IPlugin
	{
		private PluginContext _context;
		public void OnEnable(PluginContext context)
		{
			_context = context;
		}

		public void OnDisable()
		{
			
		}

		[Command(Command = "world")]
		public void WorldCommand(Player player, string world)
		{
			switch (world)
			{
				case "overworld":
					player.SendChat("Teleporting you to the Overworld!");
					_context.LevelManager.TeleportToMain(player);
					break;
				case "nether":
					player.SendChat("Teleporting you to the Nether!");
					_context.LevelManager.TeleportToLevel(player, "nether");
					break;
				default:
					player.SendChat("Unknown world! Choices: overworld, nether");
					break;
			}
		}

		[Command(Command = "tnt")]
		public void TntCommand(Player player)
		{
			var rand = new Random();
			new ActivatedTNTEntity(player.Level)
			{
				KnownPosition = player.KnownPosition,
				Fuse = (rand.Next(0, 20) + 10)
			}.SpawnEntity();
		}

		[Command(Command = "TPS")]
		public void TpsCommand(Player player)
		{
			_context.LevelManager.MainLevel.CalculateTps(player);
		}

		[Command]
		public void Hello(Player player)
		{
			player.SendChat("Hello there!"); 
		}

		[Command(Command = "save-all")]
		public void SaveAllCommand(Player player)
		{
			foreach (var lvl in _context.LevelManager.GetLevels())
			{
				lvl.SaveChunks();
			}
			_context.LevelManager.MainLevel.SaveChunks();
		}
    }
}
