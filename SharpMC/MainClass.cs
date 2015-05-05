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
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Threading;
using SharpMC.API;
using SharpMC.Crafting;
using SharpMC.Networking;
using SharpMC.Networking.Packages;
using SharpMC.Utils;
using SharpMC.Worlds;
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
				ConsoleFunctions.WriteInfoLine("Disabling plugins...");
				Globals.PluginManager.DisablePlugins();
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

			ConsoleFunctions.WriteInfoLine("Setting up some variables...");

#if DEBUG
			Globals.Debug = true;
#endif
			ConsoleFunctions.WriteInfoLine("Loading plugins...");
			Globals.PluginManager = new PluginManager();
			Globals.PluginManager.LoadPlugins();

			ConsoleFunctions.WriteInfoLine("Enabling plugins...");
			Globals.PluginManager.EnablePlugins(new List<Level>() { Globals.Level });

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