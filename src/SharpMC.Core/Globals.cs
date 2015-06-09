// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using SharpMC.Core.API;
using SharpMC.Core.Entity;
using SharpMC.Core.Networking;
using SharpMC.Core.Networking.Packages;

namespace SharpMC.Core
{
	/* Notes:
	 * Currently online-mode and compression are not working yet.
	 * We'll look into it soon.
	*/
	public class Globals
	{
		internal static int ProtocolVersion = 47;

		internal static bool UseCompression = false;

		internal static BasicListener ServerListener;
		internal static LevelManager LevelManager;
		internal static string Seed = "default";
		public static bool Debug = false;
		internal static string ProtocolName = "SharpMC 1.8";
		internal static string MCProtocolName = "Minecraft 1.8";
		public static string Motd = "";
		public static bool Offlinemode = true; //Not finished, stuck xd
		internal static bool EncryptionEnabled = true; //Only applies if offlinemode is disabled :p

		internal static Player ConsolePlayer;

		internal static PluginManager PluginManager;
		internal static RSAParameters ServerKey;
		internal static Random Rand;

		public static void BroadcastChat(string message)
		{
			foreach (var lvl in LevelManager.GetLevels())
			{
				lvl.BroadcastChat(message);
			}
			LevelManager.MainLevel.BroadcastChat(message);
		}
        public static void BroadcastChat(string message, Player sender)
        {
            foreach (var lvl in LevelManager.GetLevels())
            {
                lvl.BroadcastChat(message, sender);
            }
            LevelManager.MainLevel.BroadcastChat(message, sender);
        }

		public static int GetOnlineCount()
		{
			var count = 0;
			foreach (var lvl in LevelManager.GetLevels())
			{
				count += lvl.OnlinePlayers.Count;
			}
			count += LevelManager.MainLevel.OnlinePlayers.Count;
			return count;
		}

		#region ServerStatus

		public static int MaxPlayers { get; set; }

		private static readonly string[] ServerMotd =
		{
			"§6§l" + ProtocolName + "\n-§eNow with World Generation!",
			"§6§l" + ProtocolName + "\n-§eThis server is written by Wuppie/Kennyvv!",
			"§6§l" + ProtocolName + "\n-§eC# Powered!",
			"§6§l" + ProtocolName + "\n-§eNow supports Minecraft 1.8 (Partially)",
			"§6§l" + ProtocolName + "\n-§eEven more awesomeness!",
			"§6§l" + ProtocolName + "\n-§eKennyvv's username is PocketEdition",
			"§6§l" + ProtocolName + "\n-§eO.M.G Anvil supported!",
			"§6§l" + ProtocolName + "\n-§eBiome's supported? :o",
			"§6§l" + ProtocolName + "\n-§ePlay Minecraft, If You’ve Got The Stones",
			"§6§l" + ProtocolName + "\n-§eI Ain’t Afraid Of No Ghasts",
			"§6§l" + ProtocolName + "\n-§eYo, F*ck Creepers",
			"§6§l" + ProtocolName + "\n-§ePunching Trees Gives Me Wood",
			"§6§l" + ProtocolName + "\n-§eAny computer is a laptop if you're brave enough!",
			"§6§l" + ProtocolName + "\n-§eNothing to see here, game along..."
		};

		public static string RandomMOTD
		{
			get
			{
				if (string.IsNullOrEmpty(Motd) || Motd == "empty")
				{
					var i = new Random();
					var chosen = i.Next(0, ServerMotd.Length);
					return ServerMotd[chosen];
				}
				return Motd;
			}
		}

		#endregion

		#region Global Functions

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
            Disconnect.Broadcast("§f" + stopMsg);
	        ConsoleFunctions.WriteInfoLine("Saving all player data...");
	        foreach (var player in LevelManager.GetAllPlayers())
	        {
		        player.SavePlayer();
	        }
			OperatorLoader.SaveOperators();
            ConsoleFunctions.WriteInfoLine("Disabling plugins...");
            PluginManager.DisablePlugins();
            ConsoleFunctions.WriteInfoLine("Saving chunks...");
	        LevelManager.SaveAllChunks();
	        ServerListener.StopListenening();
	        Environment.Exit(0);
        }

		#endregion
	}
}