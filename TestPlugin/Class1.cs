using System;
using SharpMC.API;
using SharpMC.Entity;
using SharpMC.Utils;

namespace TestPlugin
{
	[Plugin]
    public class TestPlugin : IPlugin
	{
		private PluginContext Context;
		public void OnEnable(PluginContext context)
		{
			Context = context;
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
					Context.LevelManager.TeleportToMain(player);
					break;
				case "flatland":
					player.SendChat("Teleporting you to the Flatlands!");
					Context.LevelManager.TeleportToLevel(player, "flatland");
					break;
				default:
					player.SendChat("Unknown world! Choices: overworld, flatland");
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
			Context.LevelManager.MainLevel.CalculateTPS(player);
		}

		[Command]
		public void Hello(Player player)
		{
			player.SendChat("Hello there!");
		}
    }
}
