using SharpMC.API;
using SharpMC.Entity;

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

		[Command(Command = "TPS")]
		public void TpsCommand(Player player)
		{
			Context.Levels[0].CalculateTPS(player);
		}

		[Command]
		public void Hello(Player player)
		{
			player.SendChat("Hello there!");
		}
    }
}
