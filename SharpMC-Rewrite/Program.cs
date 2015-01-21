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
            LoadDebugChunks ();
            LoadPacketHandlers ();

            ConsoleFunctions.WriteInfoLine ("Starting time of day thread...");
            Globals.StartTimeOfDayTimer ();
            ConsoleFunctions.WriteInfoLine ("Started time of day thread...");

            var ClientListener = new Thread (() => new SharpMCRewrite.Networking.BasicListener ().ListenForClients ());
            ClientListener.Start ();
        }

        private static void LoadDebugChunks()
        {
            ConsoleFunctions.WriteInfoLine ("Generating chunks in debug mode...");
            Globals.WorldGen.GenerateChunkColumn (new Vector2 (0, 0));
            ConsoleFunctions.WriteInfoLine ("Done generating chunks!");
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
            Globals.Packets = temp.ToArray ();
            temp.Clear ();
            ConsoleFunctions.WriteInfoLine ("Done loading packet handlers...");
        }
    }
}
