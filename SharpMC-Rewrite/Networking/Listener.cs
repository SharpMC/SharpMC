using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace SharpMCRewrite.Networking
{
    public class BasicListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public void ListenForClients()
        {
            Globals.ServerListener.Start();
            ConsoleFunctions.WriteServerLine("Ready for connections...");
            while (true)
            {
                TcpClient client = Globals.ServerListener.AcceptTcpClient();
                ConsoleFunctions.WriteDebugLine("A new connection has been made!");

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientCommNew));
                clientThread.Start(client);
            }
        }
        private void HandleClientCommNew(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            ClientWrapper Client = new ClientWrapper(tcpClient);
            Client.MinecraftStream = new ByteBuffer (clientStream, Client);

            while (true)
            {
                MSGBuffer Buf = new MSGBuffer (Client);
                int ReceivedData = clientStream.Read (Buf.BufferedData, 0, Buf.BufferedData.Length);
                if (ReceivedData > 0)
                {
                    int length = Buf.ReadVarInt ();
                    Buf.Size = length;
                    int packid = Buf.ReadVarInt();
                    bool found = false;
                    foreach (IPacket i in Globals.Packets)
                    {
                        if (i.PacketID == packid)
                        {
                            i.Read(Client, Buf, new object[0]);
                            found = true;
                            break;
                        }
                    }
                } 
                else
                {
                    //Stop the while loop. Client disconnected!
                    break;
                }
            }
        }
    }

}
