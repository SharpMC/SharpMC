using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using SharpMC.Core.Networking;
using SharpMC.Core.Networking.Packages;
using SharpMC.PluginChannel;
using SharpMC.Core.Utils;
using SharpMC.Enums;
using SharpMC.Plugins;
using SharpMC.World;

namespace SharpMC.Core
{
	/* Notes:
	 * Currently online-mode and compression are not working yet.
	 * We'll look into it soon.
	*/
	public class Globals
	{
		internal static int ProtocolVersion = 56;
		internal static string ProtocolName = "SharpMC 15w33b";
		internal static string OfficialProtocolName = "Minecraft 15w33b";

		internal static BasicListener ServerListener;
		internal static LevelManager LevelManager;
		internal static Player ConsolePlayer;
		internal static PluginManager PluginManager;
		internal static RSAParameters ServerKey;
		internal static Random Rand;

		internal static Synchronized<ChatHandler> ChatHandler;

		internal static ClientManager ClientManager;
		internal static MessageFactory MessageFactory;

		/// <summary>
		/// Sets the chat handler.
		/// Made just for the api, so it can be safely changed to a custom handler.
		/// </summary>
		/// <param name="handler">The handler.</param>
		public void SetChatHandler(ChatHandler handler)
		{
			ChatHandler.Value = handler;
		}

		public static void BroadcastChat(string message)
		{
			BroadcastChat(new McChatMessage(message), ChatMessageType.ChatBox, null);
		}

        public static void BroadcastChat(string message, Player sender)
        {
			BroadcastChat(new McChatMessage(message), ChatMessageType.ChatBox, sender);
        }

		public static void BroadcastChat(McChatMessage message, ChatMessageType chattype, Player sender)
		{
			foreach (var lvl in LevelManager.GetLevels())
			{
				lvl.BroadcastChat(message, chattype, sender);
			}
			LevelManager.MainLevel.BroadcastChat(message, chattype, sender);
		}

		public static int GetOnlinePlayerCount()
		{
			var count = 0;
			foreach (var lvl in LevelManager.GetLevels())
			{
				count += lvl.OnlinePlayers.Count;
			}
			count += LevelManager.MainLevel.OnlinePlayers.Count;
			return count;
		}

		public static byte[] Compress(byte[] input)
		{
			using (var output = new MemoryStream())
			{
				using (var zip = new GZipStream(output, CompressionMode.Compress))
				{
					zip.Write(input, 0, input.Length);
				}
				return output.ToArray();
			}
		}

		public static byte[] Decompress(byte[] input)
		{
			using (var output = new MemoryStream(input))
			{
				using (var zip = new GZipStream(output, CompressionMode.Decompress))
				{
					var bytes = new List<byte>();
					var b = zip.ReadByte();
					while (b != -1)
					{
						bytes.Add((byte) b);
						b = zip.ReadByte();
					}
					return bytes.ToArray();
				}
			}
		}

		public static string CleanForJson(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "";
			}

			var c = '\0';
			int i;
			var len = s.Length;
			var sb = new StringBuilder(len + 4);
			string t;

			for (i = 0; i < len; i += 1)
			{
				c = s[i];
				switch (c)
				{
					case '\\':
					case '"':
						sb.Append('\\');
						sb.Append(c);
						break;
					case '/':
						sb.Append('\\');
						sb.Append(c);
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					default:
						if (c < ' ')
						{
							t = "000" + string.Format("X", c);
							sb.Append("\\u" + t.Substring(t.Length - 4));
						}
						else
						{
							sb.Append(c);
						}
						break;
				}
			}
			return sb.ToString();
		}

        public static void StopServer(string stopMsg = "Shutting down server...")
        {
            ConsoleFunctions.WriteInfoLine("Shutting down...");
			Disconnect d = new Disconnect(null);
			d.Reason = new McChatMessage("§f" + stopMsg);
			BroadcastPacket(d);
	        ConsoleFunctions.WriteInfoLine("Saving all player data...");
	        foreach (var player in LevelManager.GetAllPlayers())
	        {
		        player.SavePlayer();
	        }
			OperatorLoader.SaveOperators();
            ConsoleFunctions.WriteInfoLine("Disabling plugins...");
            PluginManager.DisablePlugins();
			ConsoleFunctions.WriteInfoLine("Saving config file...");
			Config.SaveConfig();
            ConsoleFunctions.WriteInfoLine("Saving chunks...");
	        LevelManager.SaveAllChunks();
	        ServerListener.StopListenening();
	        Environment.Exit(0);
        }

		public static void BroadcastPacket(Package packet)
		{
			foreach (var lvl in LevelManager.GetLevels())
			{
				lvl.BroadcastPacket(packet);
			}
			LevelManager.MainLevel.BroadcastPacket(packet);
		}

		/// <summary>
		/// Adds a Plugin Channel Message Handler.
		/// Returns true if it is succesfully added.
		/// Returns false if something went wrong.
		/// </summary>
		/// <param name="message">The Channel Message Handler</param>
		/// <param name="channel">The Channel</param>
		/// <returns></returns>
		public static bool RegisterPluginMessage(PluginChannel.PluginMessage message, string channel)
		{
			if (MessageFactory != null)
			{
				return MessageFactory.AddMessage(message, channel);
			}
			return false;
		}
	}
}