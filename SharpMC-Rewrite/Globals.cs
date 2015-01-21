using System;
using System.Net.Sockets;
using System.Net;
using SharpMCRewrite.Worlds;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace SharpMCRewrite
{
    public class Globals
    {
        public static IPacket[] Packets;
        public static string ProtocolName = "SharpMC 1.8";
        public static int ProtocolVersion = 47;
        public static int LastUniqueID = 0;
        public static byte Difficulty = 0;
        public static bool UseCompression = false;
        public static List<Player> Players = new List<Player> ();
        public static long TimeOfDay = 1200;
        public static long WorldAge = 0;

        public static TcpListener ServerListener = new TcpListener (IPAddress.Any, 25565);

        #region WorldGeneration
        public static FlatLandGenerator WorldGen =  new FlatLandGenerator();
        public static string LVLType = "flat";
        #endregion

        #region ServerStatus
        public static int MaxPlayers = 10;
        public static int PlayersOnline = 0;

        public static string[] ServerMOTD = new string[] 
        {
            "§6§lSharpMC\n-§eComplete rewrite!",
            "§6§lSharpMC\n-§eThis server is written by Wuppie/Kennyvv!",
            "§6§lSharpMC\n-§eC# Powered!",
            "§6§lSharpMC\n-§eNow supports Minecraft 1.8 (Partially)"
        };

        public static string RandomMOTD
        {
            get
            {
                Random i = new Random ();
                int Chosen = i.Next (0, ServerMOTD.Length);
                return ServerMOTD [Chosen];
            }
        }
        #endregion

        #region TickTimer
        private static Thread TimerThread = new Thread (() => StartTimeTimer ());

        public static void StartTimeOfDayTimer()
        {
            TimerThread.Start ();
        }

        public static void StopTimeOfDayTimer()
        {
            TimerThread.Abort ();
            TimerThread = new Thread (() => StartTimeTimer());
        }

        static System.Timers.Timer kTimer = new System.Timers.Timer();

        private static void StartTimeTimer()
        {
            kTimer.Elapsed += new ElapsedEventHandler(RunTick);
            kTimer.Interval = 1000;
            kTimer.Start();
        }

        private static void StopTimeTimer()
        {
            kTimer.Stop ();
        }

        private static void RunTick(object source, ElapsedEventArgs e)
        {
            if (TimeOfDay < 24000)
            {
                TimeOfDay += 20;
            }
            else
            {
                TimeOfDay = 0;
                WorldAge++;
            }

            foreach (Player i in Globals.Players)
            {
                new TimeUpdate ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[0]);
            }
        }
        #endregion

        public static void RemovePlayer(Player p)
        {
            Players.Remove (p);
            BroadcastMessage ("§e" + p.Username + " has left the server!");
        }

        public static void AddPlayer(Player p)
        {
            Players.Add (p);
            BroadcastMessage ("§e" + p.Username + " has joined the server!");
        }

        public static void BroadcastMessage(string Message)
        {
            foreach(Player i in Players)
            {
                new ChatMessage ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[] { Message });
            }
            ConsoleFunctions.WriteInfoLine ("Chat: " + Message);
        }

    }
}

