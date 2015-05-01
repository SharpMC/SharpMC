using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Threading;
using SharpMC.Networking;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds.Anvil;
using SharpMC.Worlds.Flatland;
using SharpMC.Worlds.Standard;

namespace SharpMC
{
	internal class MainClass
	{
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
		public static void Main(string[] args)
		{
			var currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += UnhandledException;

			Console.Title = Globals.ProtocolName;

			Config.ConfigFile = "server.properties";
			Config.InitialValue = new[]
			{
				"#DO NOT REMOVE THIS LINE - SharpMC Config",
				"Port=25565",
				"MaxPlayers=10",
				"LevelType=standard",
				"WorldName=world",
				"Debug=false",
				"Seed=",
				"Motd=",
			};
			Config.Check();

			Console.CancelKeyPress += delegate
			{
				ConsoleFunctions.WriteInfoLine("Shutting down...");
				Disconnect.Broadcast("§fServer shutting down...");
				ConsoleFunctions.WriteInfoLine("Saving chunks...");
				Globals.Level.SaveChunks();
			};

			ConsoleFunctions.WriteInfoLine("Loading config file...");
			Globals.MaxPlayers = Config.GetProperty("MaxPlayers", 10);
			var Lvltype = Config.GetProperty("LevelType", "Experimental");
			switch (Lvltype.ToLower())
			{
				case "flatland":
					Globals.Level = new FlatLandLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "standard":
					Globals.Level = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "anvil":
					Globals.Level = new AnvilLevel(Config.GetProperty("WorldName", "world"));
					break;
				default:
					Globals.Level = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
			}
			Globals.Seed = Config.GetProperty("Seed", "SharpieCraft");

			Globals.Motd = Config.GetProperty("motd", "");

			int port = Config.GetProperty("port", 25565);
			if (port != 25565) Globals.ServerListener = new TcpListener(IPAddress.Any, port);

			Globals.Debug = Config.GetProperty("debug", false);

			ConsoleFunctions.WriteInfoLine("Checking files...");

			if (!Directory.Exists(Globals.Level.LvlName))
				Directory.CreateDirectory(Globals.Level.LvlName);

			var ClientListener = new Thread(() => new BasicListener().ListenForClients());
			ClientListener.Start();
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			var e = (Exception) args.ExceptionObject;
			ConsoleFunctions.WriteErrorLine("An unhandled exception occured! Error message: " + e.Message);
		}
	}
}