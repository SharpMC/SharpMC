using System;
using SharpMCRewrite.Networking;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using SharpMCRewrite.Packets;

namespace SharpMCRewrite
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            LoadPacketHandlers ();
            Thread ClientListener = new Thread (() => new SharpMCRewrite.Networking.BasicListener ().ListenForClients ());
            ClientListener.Start ();
        }

        private static void LoadPacketHandlers()
        {
            List<IPacket> temp = new List<IPacket> ();
            temp.Add (new Ping());
            temp.Add (new Handshake());

            Globals.Packets = temp.ToArray ();
            temp.Clear ();
        }
    }
}
