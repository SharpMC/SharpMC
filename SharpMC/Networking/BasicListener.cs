using System;
using System.Net.Sockets;
using System.Threading;
using SharpMC.Classes;
using SharpMC.Networking.Packages;

namespace SharpMC.Networking
{
	public class BasicListener
	{
		public void ListenForClients()
		{
			Globals.ServerListener.Start();
			ConsoleFunctions.WriteServerLine("Ready for connections...");
			ConsoleFunctions.WriteInfoLine("To shutdown the server safely press CTRL+C");
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
					var buf = new MSGBuffer(Client);
					var receivedData = clientStream.Read(buf.BufferedData, 0, buf.BufferedData.Length);
					if (receivedData > 0)
					{
						var length = buf.ReadVarInt();
						buf.Size = length;
						var packid = buf.ReadVarInt();

						if (!new PackageFactory(Client, buf).Handle(packid))
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
					Client.ThreadPool.KillAllThreads();
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
			Client.ThreadPool.KillAllThreads();
			Client.StopKeepAliveTimer();

			if (Client.Player != null)
			{
				Globals.Level.RemovePlayer(Client.Player);
				Globals.Level.BroadcastPlayerRemoval(Client);
			}
			Client.TcpClient.Close();
			Thread.CurrentThread.Abort();
		}
	}
}