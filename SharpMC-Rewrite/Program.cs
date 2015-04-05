using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using SharpMCRewrite.Networking;
using SharpMCRewrite.Networking.Packages;
using SharpMCRewrite.Utils;
using SharpMCRewrite.Worlds;
using SharpMCRewrite.Worlds.ExperimentalV2;
using SharpMCRewrite.Worlds.Flatland;

namespace SharpMCRewrite
{
	internal class MainClass
	{
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
		public static void Main(string[] args)
		{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				currentDomain.UnhandledException += UnhandledException;

				Config.ConfigFile = "server.properties";
				Config.InitialValue = new[]
				{
					"#DO NOT REMOVE THIS LINE - SharpMC Config",
					"MaxPlayers=10",
					"LevelType=anvil",
					"WorldName=world",
					"Seed=SharpieCraft"
				};
				Config.Check();

				Console.CancelKeyPress += delegate
				{
					ConsoleFunctions.WriteInfoLine("Shutting down...");
					//Globals.Level.BroadcastPacket(new Disconnect(), new object[] { "Server shutting down!" });
					Disconnect.Broadcast("§fServer shutting down...");
					ConsoleFunctions.WriteInfoLine("Saving chunks...");
					Globals.Level.SaveChunks();
				};

				ConsoleFunctions.WriteInfoLine("Loading config file...");
				Globals.MaxPlayers = Config.GetProperty("MaxPlayers", 10);
				var Lvltype = Config.GetProperty("LevelType", "Experimental");
				switch (Lvltype.ToLower())
				{
					case "flatLand":
						Globals.Level = new FlatLandLevel(Config.GetProperty("WorldName", "world"));
						break;
					case "normal":
						Globals.Level = new ExperimentalV2Level(Config.GetProperty("WorldName", "world"));
						break;
					case "anvil":
						Globals.Level = new AnvilLevel(Config.GetProperty("WorldName", "world"));
						break;
					default:
						Globals.Level = new ExperimentalV2Level(Config.GetProperty("WorldName", "world"));
						break;
				}
				Globals.Seed = Config.GetProperty("Seed", "SharpieCraft");

				ConsoleFunctions.WriteInfoLine("Generating chunks...");
				Globals.Level.Generator.GenerateChunks(8*16, 0, 0, new Dictionary<Tuple<int, int>, ChunkColumn>(), null);

				ConsoleFunctions.WriteInfoLine("Checking files...");

				if (!Directory.Exists(Globals.Level.LVLName))
					Directory.CreateDirectory(Globals.Level.LVLName);

				var ClientListener = new Thread(() => new BasicListener().ListenForClients());
				ClientListener.Start();
		}

		static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			ConsoleFunctions.WriteErrorLine("An unhandled exception occured! Error message: " + e.Message);
		}
	}
}