using System;
using System.Net.Sockets;
using System.Net;

namespace SharpMCRewrite
{
    public class Globals
    {
        public static IPacket[] Packets;
        public static string ProtocolName = "SharpMC 1.8";
        public static int ProtocolVersion = 47;
        public static int MaxPlayers = 10;
        public static int PlayersOnline = 0;
        public static string ServerMOTD = "SharpMC - Complete rewrite!";

        public static TcpListener ServerListener = new TcpListener (IPAddress.Any, 25565);
    }
}

