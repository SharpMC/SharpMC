using System.Net.Sockets;
using System.Threading;
using System;

namespace SharpMCRewrite.Networking
{
    public class BasicListener
    {
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

            while (true)
            {
                try
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
                            if (i.PacketID == packid && i.IsPlayePacket == Client.PlayMode)
                            {
                                i.Read(Client, Buf, new object[0]);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            ConsoleFunctions.WriteWarningLine ("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
                        }
                    } 
                    else
                    {
                        //Stop the while loop. Client disconnected!
                        break;
                    }
                }
                catch(Exception ex)
                {
                    //Exception, disconnect!
                    ConsoleFunctions.WriteDebugLine ("Error: \n" + ex);
                    new Disconnect ().Write (Client, new MSGBuffer (Client), new object[] { "§4SharpMC\n§fServer threw an exception!" });
                    break;
                }
            }
            //Close the connection with the client. :)
            Client.StopKeepAliveTimer ();

            if (Client.Player != null)
                Globals.RemovePlayer (Client.Player);

            Client.TCPClient.Close ();
        }
    }

}
