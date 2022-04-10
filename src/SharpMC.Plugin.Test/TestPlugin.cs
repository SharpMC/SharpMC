using System;
using SharpMC.API.Attributes;
using SharpMC.API.Entities;
using SharpMC.API.Enums;
using SharpMC.API.Plugins;

namespace SharpMC.Plugin.Test
{
    [Plugin]
    public class TestPlugin : IPlugin
    {
        private IPluginContext _context;

        public void OnEnable(IPluginContext context)
        {
            _context = context;
        }

        public void OnDisable()
        {
        }

        [Command(Command = "o")]
        public void OptionTest(IPlayer player, string test, string test2 = "")
        {
            player.SendChat($"Test={test}, Test2={test2}");
        }

        [Command(Command = "myinfo")]
        public void MyInfo(IPlayer player, bool console = false)
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