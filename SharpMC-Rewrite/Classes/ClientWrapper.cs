using System;
using System.Net.Sockets;

namespace SharpMCRewrite
{
    public class ClientWrapper
    {
        public TcpClient TCPClient;
        public Player Player;
        public bool PlayMode = false;
        public ByteBuffer MinecraftStream;

        public ClientWrapper(TcpClient client)
        {
            TCPClient = client;
        }

        public void SendData(byte[] Data, int Length)
        {
            NetworkStream a = TCPClient.GetStream ();
            a.Write (Data, 0, Length);
            a.Flush ();
        }
       
        public void SendData(byte[] Data, int Offset, int Length)
        {
            NetworkStream a = TCPClient.GetStream ();
            a.Write (Data, Offset, Length);
            a.Flush ();
        }

        public void SendData(byte[] Data)
        {
            NetworkStream a = TCPClient.GetStream ();
            a.Write (Data, 0, Data.Length);
            a.Flush ();
        }
    }
}

