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
using System.IO;
using System.Security.Permissions;
using System.Threading;
using SharpMC.API;
using SharpMC.Crafting;
using SharpMC.Entity;
using SharpMC.Enums;
using SharpMC.Networking;
using SharpMC.Utils;
using SharpMC.Worlds;
using SharpMC.Worlds.Anvil;
using SharpMC.Worlds.Flatland;
using SharpMC.Worlds.Nether;
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

			Globals.Rand = new Random();
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
				"Online-mode=false",
				"Seed=",
				"Motd="
			};
			Config.Check();

			Console.CancelKeyPress += delegate
			{
				Globals.StopServer("Server shutting down!");
			};

			ConsoleFunctions.WriteInfoLine("Loading config file...");
			Globals.MaxPlayers = Config.GetProperty("MaxPlayers", 10);
			var lvltype = Config.GetProperty("LevelType", "Experimental");
			Level lvl;
			switch (lvltype.ToLower())
			{
				case "flatland":
					lvl = new FlatLandLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "standard":
					lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "anvil":
					lvl = new AnvilLevel(Config.GetProperty("WorldName", "world"));
					break;
				default:
					lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
			}
			Globals.LevelManager = new LevelManager(lvl);
			Globals.LevelManager.AddLevel("nether", new NetherLevel("nether")); //Initiate the 'nether'

			Globals.Seed = Config.GetProperty("Seed", "SharpieCraft");

			Globals.Motd = Config.GetProperty("motd", "");

			Globals.Debug = Config.GetProperty("debug", false);

			Globals.Offlinemode = !Config.GetProperty("Online-mode", false);

			OperatorLoader.LoadOperators();

			Globals.ConsolePlayer = new Player(Globals.LevelManager.MainLevel)
			{
				Username = "Console",
				Wrapper = new ClientWrapper(null),
				Uuid = Guid.NewGuid().ToString(),
				Gamemode = Gamemode.Spectator,
			};
			Globals.ConsolePlayer.Wrapper.Player = Globals.ConsolePlayer;
			Globals.ConsolePlayer.IsOperator = true;
			
			ConsoleFunctions.WriteInfoLine("Checking files...");

			if (!Directory.Exists(Globals.LevelManager.MainLevel.LvlName))
				Directory.CreateDirectory(Globals.LevelManager.MainLevel.LvlName);

			if (!Directory.Exists("Players"))
				Directory.CreateDirectory("Players");

			ConsoleFunctions.WriteInfoLine("Setting up some variables...");
			Globals.ServerKey = PacketCryptography.GenerateKeyPair();
#if DEBUG
			Globals.Debug = true;
#endif

			//ConsoleFunctions.WriteInfoLine("Loading default crafting recipes...");
			//RecipeFactory.Init();

			ConsoleFunctions.WriteInfoLine("Loading plugins...");
			Globals.PluginManager = new PluginManager();
			Globals.PluginManager.LoadPlugins();

			ConsoleFunctions.WriteInfoLine("Enabling plugins...");
			Globals.PluginManager.EnablePlugins(Globals.LevelManager);

			Globals.ServerListener = new BasicListener();

			new Thread(() => Globals.ServerListener.ListenForClients()).Start();
			new Thread(() => new ConsoleCommandHandler().WaitForCommand()).Start();
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			var e = (Exception) args.ExceptionObject;
			ConsoleFunctions.WriteErrorLine("An unhandled exception occured! Error message: " + e.Message);
		}
	}
}