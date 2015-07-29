using SharpMC.Core.Entity;
using SharpMC.Core.Enums;

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
			player.Kick(reason);
		}

		public static void KickPlayer(Player player)
		{
			player.Kick("You were kicked from the server.");
		}
	}
}
