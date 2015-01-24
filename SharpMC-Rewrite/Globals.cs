using System;
using System.Net.Sockets;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;

namespace SharpMCRewrite
{
    public class Globals
    {
        #region ServerStatus
        public static int MaxPlayers = 10;

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
        public static IPacket[] Packets;
        public static string ProtocolName = "SharpMC 1.8";
        public static int ProtocolVersion = 47;
        public static int LastUniqueID = 0;
        public static bool UseCompression = false;
        public static TcpListener ServerListener = new TcpListener (IPAddress.Any, 25565);
        public static ILevel Level;
        public static ConfigFileReader ConfigParser;


        #region Global Functions

        public static byte[] Compress(byte[] input)
        {
            using (MemoryStream output = new MemoryStream()) 
            {
                using (GZipStream zip = new GZipStream(output,CompressionMode.Compress) ) 
                {
                    zip.Write(input, 0, input.Length);
                }
                return output.ToArray();
            }
        }

        public static byte[] Decompress(byte[] input)
        {
            using (MemoryStream output = new MemoryStream (input))
            {
                using (GZipStream zip = new GZipStream (output, CompressionMode.Decompress))
                {
                    List<byte> bytes = new List<byte> ();
                    int b = zip.ReadByte ();
                    while (b != -1)
                    {
                        bytes.Add ((byte)b);
                        b = zip.ReadByte ();

                    }
                    return bytes.ToArray ();
                }
            }
        }

        #endregion
    }
}

