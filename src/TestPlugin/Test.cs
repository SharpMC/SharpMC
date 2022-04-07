using System;
using SharpMC;
using SharpMC.Enums;
using SharpMC.Plugins.API;

namespace TestPlugin
{
	[Plugin]
    public class Test : IPlugin
	{
		private PluginContext _context;
		public void OnEnable(PluginContext context)
		{
			_context = context;
		}

		public void OnDisable()
		{
			
		}

		[Command(Command = "o")]
		public void OptionTest(Player player, string test, string test2 = "")
		{
			player.SendChat(String.Format("Test={0}, Test2={1}", test, test2));
		}

		[Command(Command = "myinfo")]
		public void MyInfo(Player player, bool console = false)
		{
			player.SendChat("====Start Debug Info====", ChatColor.Yellow);
			player.SendChat("Operator Status: " + player.IsOperator, ChatColor.Yellow);
			player.SendChat("Username: " + player.Username, ChatColor.Yellow);
			player.SendChat("UUID: " + player.Uuid, ChatColor.Yellow);
			player.SendChat("Gamemode: " + player.Gamemode, ChatColor.Yellow);
			player.SendChat("====End of Debug Info====", ChatColor.Yellow);

			if (console)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("====Start Debug Info====");
				Console.WriteLine("Operator Status: " + player.IsOperator);
				Console.WriteLine("Username: " + player.Username);
				Console.WriteLine("UUID: " + player.Uuid);
				Console.WriteLine("Gamemode: " + player.Gamemode);
				Console.WriteLine("====End of Debug Info====");
				Console.ResetColor();
			}
		}
    }
}