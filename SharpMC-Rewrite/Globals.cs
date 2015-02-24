using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;

namespace SharpMCRewrite
{
	public class Globals
	{
		public static string ProtocolName = "SharpMC 1.8";
		public static int ProtocolVersion = 47;
		public static int LastUniqueID = 0;
		public static bool UseCompression = false; //Please note, this is not working yet!
		public static TcpListener ServerListener = new TcpListener(IPAddress.Any, 25565);
		public static Level Level;
		public static string Seed = "default";
		public static NoiseGenerator NoiseGenerator = NoiseGenerator.Simplex;

		#region ServerStatus

		public static int MaxPlayers = 10;

		public static string[] ServerMOTD =
		{
			"§6§lSharpMC\n-§eNow with World Generation!",
			"§6§lSharpMC\n-§eThis server is written by Wuppie/Kennyvv!",
			"§6§lSharpMC\n-§eC# Powered!",
			"§6§lSharpMC\n-§eNow supports Minecraft 1.8 (Partially)",
			"§6§lSharpMC\n-§eEven more awesomeness!",
			"§6§lSharpMC\n-§eKennyvv's username is PocketEdition"
		};

		public static string RandomMOTD
		{
			get
			{
				var i = new Random();
				var Chosen = i.Next(0, ServerMOTD.Length);
				return ServerMOTD[Chosen];
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

	public enum NoiseGenerator
	{
		Simplex,
		OpenSimplex,
		Perlin
	}
}