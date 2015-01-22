using System;
using System.Threading;
using System.Collections.Generic;
using SharpMCRewrite.Packets;

namespace SharpMCRewrite
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Globals.ConfigParser = new ConfigFileReader ("server.properties");
            ConsoleFunctions.WriteInfoLine ("Loading config file...");
            Globals.MaxPlayers = Globals.ConfigParser.ReadInt ("MaxPlayers");
            LoadPacketHandlers ();
            var ClientListener = new Thread (() => new SharpMCRewrite.Networking.BasicListener ().ListenForClients ());
            ClientListener.Start ();
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
