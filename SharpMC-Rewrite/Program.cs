using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MiNET.Utils;
using SharpMCRewrite.Networking;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Worlds.Experimental;
using SharpMCRewrite.Worlds.ExperimentalV2;
using SharpMCRewrite.Worlds.Nether;

namespace SharpMCRewrite
{
	internal class MainClass
	{
		public static void Main(string[] args)
		{
			ConfigParser.ConfigFile = "server.properties";
			ConfigParser.InitialValue = new[]
			{
				"#DO NOT REMOVE THIS LINE - SharpMC Config",
				"MaxPlayers=10",
				"LevelType=Experimental",
				"WorldName=world",
				"Seed=SharpieCraft"
			};
			ConfigParser.Check();

			ConsoleFunctions.WriteInfoLine("Loading config file...");
			Globals.MaxPlayers = ConfigParser.GetProperty("MaxPlayers", 10);
			var Lvltype = ConfigParser.GetProperty("LevelType", "Experimental");
			switch (Lvltype)
			{
				case "FlatLand":
					Globals.Level = new FlatLandLevel(ConfigParser.GetProperty("WorldName", "world"));
					break;
				case "Experimental":
					Globals.Level = new ExperimentalLevel(ConfigParser.GetProperty("WorldName", "world"));
					break;
				case "Hell":
				case "Nether":
					Globals.Level = new NetherLevel(ConfigParser.GetProperty("WorldName", "world"));
					break;
				default:
					Globals.Level = new ExperimentalV2Level(ConfigParser.GetProperty("WorldName", "world"));
					break;
			}
			Globals.Seed = ConfigParser.GetProperty("Seed", "SharpieCraft");

			ConsoleFunctions.WriteInfoLine("Checking files...");

			if (!Directory.Exists(Globals.Level.LVLName))
				Directory.CreateDirectory(Globals.Level.LVLName);

			LoadPacketHandlers();
			var ClientListener = new Thread(() => new BasicListener().ListenForClients());
			ClientListener.Start();

			Console.CancelKeyPress += delegate
			{
				ConsoleFunctions.WriteInfoLine("Shutting down...");
				//Globals.Level.BroadcastPacket(new Disconnect(), new object[] { "Server shutting down!" });
				Disconnect.Broadcast("§fServer shutting down...");
				ConsoleFunctions.WriteInfoLine("Saving chunks...");
				Globals.Level.SaveChunks();
			};
		}

		private static void LoadPacketHandlers()
		{
			ConsoleFunctions.WriteInfoLine("Loading packet handlers..");
			var temp = new List<IPacket>();
			temp.Add(new Ping());
			temp.Add(new ClientSettings());
			temp.Add(new OnGround());
			temp.Add(new PlayerAnimation());
			temp.Add(new PlayerDigging());
			Globals.Packets = temp.ToArray();
			temp.Clear();
			ConsoleFunctions.WriteInfoLine("Done loading packet handlers...");
		}
	}
}