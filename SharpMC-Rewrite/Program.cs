using System;
using System.Threading;
using System.Collections.Generic;
using SharpMCRewrite.Packets;
using System.IO;

namespace SharpMCRewrite
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Globals.ConfigParser = new ConfigFileReader ("server.properties");
            ConsoleFunctions.WriteInfoLine ("Loading config file...");
            Globals.MaxPlayers = Globals.ConfigParser.ReadInt ("MaxPlayers");
            string Lvltype = Globals.ConfigParser.ReadString ("Leveltype");
            switch (Lvltype)
            {
                case "FlatLand":
                    Globals.Level = new FlatLandLevel(Globals.ConfigParser.ReadString ("WorldName"));
                    break;
                default:
                    Globals.Level = new FlatLandLevel(Globals.ConfigParser.ReadString ("WorldName"));
                    break;
            }
            ConsoleFunctions.WriteInfoLine ("Checking files...");

            if (!Directory.Exists (Globals.Level.LVLName))
                Directory.CreateDirectory (Globals.Level.LVLName);

            LoadPacketHandlers ();
            var ClientListener = new Thread (() => new SharpMCRewrite.Networking.BasicListener ().ListenForClients ());
            ClientListener.Start ();

            Console.CancelKeyPress += delegate
            {
                ConsoleFunctions.WriteInfoLine("Shutting down...");
                Globals.Level.BroadcastPacket(new Disconnect(), new object[] { "Server shutting down!" });
                ConsoleFunctions.WriteInfoLine("Saving chunks...");
                Globals.Level.SaveChunks();
            };
        }

        private static void LoadPacketHandlers()
        {
            ConsoleFunctions.WriteInfoLine ("Loading packet handlers..");
            var temp = new List<IPacket> ();
            temp.Add (new Ping());
            temp.Add (new Handshake());
            temp.Add (new KeepAlive());
            temp.Add (new PlayerPosition ());
            temp.Add (new PlayerPositionAndLook ());
            temp.Add (new PlayerLook ());
            temp.Add (new ClientSettings ());
            temp.Add (new OnGround ());
            temp.Add (new ChatMessage ());
            temp.Add (new PlayerDigging ());
            temp.Add (new PlayerAnimation ());
            Globals.Packets = temp.ToArray ();
            temp.Clear ();
            ConsoleFunctions.WriteInfoLine ("Done loading packet handlers...");
        }
    }
}
