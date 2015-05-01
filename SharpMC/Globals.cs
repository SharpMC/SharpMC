using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using SharpMC.Worlds;

namespace SharpMC
{
	public class Globals
	{
		public static int ProtocolVersion = 47;
		public static bool UseCompression = false; //Please note, this is not working yet! (not planning on adding any where soon)
		public static TcpListener ServerListener = new TcpListener(IPAddress.Any, 25565);
		public static Level Level;
		public static string Seed = "default";
		public static bool SupportSharpMC = false; //Enable for player list ads xD
		public static bool Debug = false;
		public static string ProtocolName = "SharpMC 1.8";
		public static string MCProtocolName = "Minecraft 1.8";
		public static string Motd = "";
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
				if (String.IsNullOrEmpty(Motd) || Motd == "empty")
				{
					var i = new Random();
					var chosen = i.Next(0, ServerMotd.Length);
					return ServerMotd[chosen];
				}
				else
				{
					return Motd;
				}
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

		#endregion
	}
}