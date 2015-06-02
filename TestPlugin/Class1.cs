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

		[Command(Command = "o")]
		public void OptionTest(Player player, string test, string test2 = "")
		{
			player.SendChat("Huray :)");
		}
    }
}
