// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking
{
	public class BasicListener
	{
		private TcpListener _serverListener = new TcpListener(IPAddress.Any, 25565);
		private bool _listening = false;

		public void ListenForClients()
		{
			var port = Config.GetProperty("port", 25565);
			if (port != 25565) _serverListener = new TcpListener(IPAddress.Any, port);

			_serverListener.Start();
			_listening = true;
			ConsoleFunctions.WriteServerLine("Ready for connections...");
			ConsoleFunctions.WriteInfoLine("To shutdown the server safely press CTRL+C");
			while (_listening)
			{
				var client = _serverListener.AcceptTcpClient();
				ConsoleFunctions.WriteDebugLine("A new connection has been made!");

				new Task((() => { HandleClientCommNew(client); })).Start(); //Task instead of Thread
			}
		}

		public void StopListenening()
		{
			_listening = false;
			_serverListener.Stop();
		}

		private int ReadVarInt(NetworkStream stream)
		{
			var value = 0;
			var size = 0;
			int b;

			//value |= (val & 0x7F) << (size++*7);

			while (((b = stream.ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++ * 7);
				if (size > 5)
				{
					throw new IOException("VarInt too long. Hehe that's punny.");
				}
			}
			return value | ((b & 0x7F) << (size * 7));
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
					while (!clientStream.DataAvailable)
						Thread.Sleep(1);

					int dlength = ReadVarInt(clientStream);
					var buffie = new byte[dlength];
					int receivedData;
					receivedData = clientStream.Read(buffie, 0, buffie.Length);

					if (receivedData > 0)
					{
						var buf = new DataBuffer(Client);

						if (Client.Decrypter != null)
						{
							var date = new byte[4096];
							Client.Decrypter.TransformBlock(buffie, 0, buffie.Length, date, 0);
							buf.BufferedData = date;
						}
						else
						{
							buf.BufferedData = buffie;
						}

						buf.BufferedData = buffie;

						//var length = buf.ReadVarInt();
						//buf.Size = length;
						//buf.SetDataSize(length + buf.GetReadDataLength()); //Resize the array
						buf.Size = dlength;
						var packid = buf.ReadVarInt();

						if (!new PackageFactory(Client, buf).Handle(packid))
						{
							ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
						}
						buf.Dispose();
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
				Client.Player.SavePlayer();
				Client.Player.Level.RemovePlayer(Client.Player);
				Client.Player.Level.BroadcastPlayerRemoval(Client);
			}
			Client.TcpClient.Close();
			Thread.CurrentThread.Abort();
		}
		
	/*	private void HandleClientNetwork(TcpClient client)
		{
			var wrapper = new ClientWrapper(client);
			while (true)
			{
				try
				{
					DataBuffer stream = new DataBuffer(client.Client);
					int packetLength = stream.ReadVarInt();
					stream.TotalDataLength = packetLength;

					int packetId = stream.ReadVarInt();
					
					if (!new PackageFactory(wrapper, stream).Handle(packetId))
					{
						ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packetId.ToString("X2") + "\"");
					}
					stream.Dispose();
				}
				catch(Exception ex)
				{
					wrapper.ThreadPool.KillAllThreads();
					//Exception, disconnect!
					ConsoleFunctions.WriteDebugLine("Error: \n" + ex);
					new Disconnect(wrapper)
					{
						Reason = "§4SharpMC\n§fServer threw an exception!\n\nFor the nerdy people: \n" + ex.Message
					}.Write();
					break;
				}
			}
			//Close the connection with the client. :)
			wrapper.ThreadPool.KillAllThreads();
			wrapper.StopKeepAliveTimer();

			if (wrapper.Player != null)
			{
				wrapper.Player.SavePlayer();
				wrapper.Player.Level.RemovePlayer(wrapper.Player);
				wrapper.Player.Level.BroadcastPlayerRemoval(wrapper);
			}
			wrapper.TcpClient.Close();
			Thread.CurrentThread.Abort();
		}*/
	}
}