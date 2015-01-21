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
            var ClientListener = new Thread (() => new SharpMCRewrite.Networking.BasicListener ().ListenForClients ());
            ClientListener.Start ();
        }

        private static void LoadDebugChunks()
        {
            ConsoleFunctions.WriteInfoLine ("Generating chunks in debug mode...");
            Globals.ChunkColums.Add (Globals.WorldGen.GenerateChunkColumn (new Vector2 (0, 0)));
         /*   int r = 49; //Radius. (13*4 = 52) and we need 49 Chunks for a player to spawn. So this should be fine :)
            int ox = 0, oy = 0; //Middle point

            int done = 0;
            for (int x = -r; x < r ; x++)
            {
                int height = (int)Math.Sqrt(r * r - x * x);

                for (int y = -height; y < height; y++)
                {
                    Globals.ChunkColums.Add (Globals.WorldGen.GenerateChunkColumn (new Vector2 (x + ox, y + oy)));
                    done++;
                    if (done == r)
                        break;
                }
                if (done == r)
                    break;
            }*/
            ConsoleFunctions.WriteInfoLine ("Done generating chunks! DEBUG: " + Globals.ChunkColums.Count);
        }

        private static void LoadPacketHandlers()
        {
            ConsoleFunctions.WriteInfoLine ("Loading packet handlers..");
            var temp = new List<IPacket> ();
            temp.Add (new Ping());
            temp.Add (new Handshake());
            temp.Add (new KeepAlive());
            Globals.Packets = temp.ToArray ();
            temp.Clear ();
            ConsoleFunctions.WriteInfoLine ("Done loading packet handlers...");
        }
    }
}
