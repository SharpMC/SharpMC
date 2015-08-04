using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;

namespace SharpMC.API
{
	public class PlayerUtils
	{
		public static void SendChatMessage(Player player, string message)
		{
			player.SendChat(message);
		}

		public static void SendChatMessage(Player player, string message, ChatColor color)
		{
			player.SendChat(message, color);
		}

		public static void KickPlayer(Player player, string reason)
		{
			//player.Kick(reason);
			KickPlayer(player, new McChatMessage(reason));
		}

		public static void KickPlayer(Player player)
		{
			//player.Kick("You were kicked from the server.");
			KickPlayer(player, new McChatMessage("You were kicked from the server."));
		}

		public static void KickPlayer(Player player, McChatMessage message)
		{
			player.Kick(message);
		}
	}
}
