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
using System.Net;
using System.Net.Sockets;
using System.Text;
using SharpMC.API;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC
{
	internal class Globals
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

		public static bool Offlinemode = true; //Not finished, stuck xd
		public static PluginManager PluginManager;

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

		public static string CleanForJson(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "";
			}

			char c = '\0';
			int i;
			int len = s.Length;
			StringBuilder sb = new StringBuilder(len + 4);
			String t;

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
							t = "000" + String.Format("X", c);
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

		#endregion
	}
}