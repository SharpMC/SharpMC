using System;
using System.Net.Sockets;
using System.Threading;
using SharpMCRewrite.Classes;
using SharpMCRewrite.Networking.Packages;

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
				var client = Globals.ServerListener.AcceptTcpClient();
				ConsoleFunctions.WriteDebugLine("A new connection has been made!");

				var clientThread = new Thread(HandleClientCommNew);
				clientThread.Start(client);
			}
		}

		private void HandleClientCommNew(object client)
		{
			var tcpClient = (TcpClient) client;
			var clientStream = tcpClient.GetStream();
			var Client = new ClientWrapper(tcpClient);

			while (true)
			{
				try
				{
					var Buf = new MSGBuffer(Client);
					var ReceivedData = clientStream.Read(Buf.BufferedData, 0, Buf.BufferedData.Length);
					if (ReceivedData > 0)
					{
						var length = Buf.ReadVarInt();
						Buf.Size = length;
						var packid = Buf.ReadVarInt();

						if (!new PackageFactory(Client, Buf).Handle(packid))
						{
							ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
						}
					}
					else
					{
						//Stop the while loop. Client disconnected!
						break;
					}
				}
				catch (Exception ex)
				{
					//Exception, disconnect!
					ConsoleFunctions.WriteDebugLine("Error: \n" + ex);
					new Disconnect(Client)
					{
						Reason = "§4SharpMC\n§fServer threw an exception!\n\nFor the nerdy people: \n" + ex.Message
					}.Write();
					break;
				}
			}
			//Close the connection with the client. :)
			Client.StopKeepAliveTimer();

			if (Client.Player != null)
			{
				Globals.Level.RemovePlayer(Client.Player);
				Globals.Level.BroadcastPlayerRemoval(Client);
			}
			Client.TCPClient.Close();
		}
	}
}