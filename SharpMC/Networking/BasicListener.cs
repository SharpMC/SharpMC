#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Networking
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;
	using System.Threading.Tasks;

	using SharpMC.Networking.Packages;
	using SharpMC.Utils;

	public class BasicListener
	{
		private bool _listening;

		private TcpListener _serverListener = new TcpListener(IPAddress.Any, 25565);

		public void ListenForClients()
		{
			var port = Config.GetProperty("port", 25565);
			if (port != 25565)
			{
				this._serverListener = new TcpListener(IPAddress.Any, port);
			}

			this._serverListener.Start();
			this._listening = true;
			ConsoleFunctions.WriteServerLine("Ready for connections...");
			ConsoleFunctions.WriteInfoLine("To shutdown the server safely press CTRL+C");
			while (this._listening)
			{
				var client = this._serverListener.AcceptTcpClient();
				ConsoleFunctions.WriteDebugLine("A new connection has been made!");

				new Task(() => { this.HandleClientCommNew(client); }).Start(); // Task instead of Thread
			}
		}

		public void StopListenening()
		{
			this._listening = false;
			this._serverListener.Stop();
		}

		private void HandleClientCommNew(object client)
		{
			var tcpClient = (TcpClient)client;
			var clientStream = tcpClient.GetStream();
			var Client = new ClientWrapper(tcpClient);

			while (true)
			{
				try
				{
					var buffie = new byte[4096];
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

						var length = buf.ReadVarInt();
						buf.Size = length;
						var packid = buf.ReadVarInt();

						if (!new PackageFactory(Client, buf).Handle(packid))
						{
							ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
						}

						buf.Dispose();
					}
					else
					{
						// Stop the while loop. Client disconnected!
						break;
					}
				}
				catch (Exception ex)
				{
					Client.ThreadPool.KillAllThreads();

					// Exception, disconnect!
					ConsoleFunctions.WriteDebugLine("Error: \n" + ex);
					new Disconnect(Client)
						{
							Reason = "§4SharpMC\n§fServer threw an exception!\n\nFor the nerdy people: \n" + ex.Message
						}.Write();
					break;
				}
			}

			// Close the connection with the client. :)
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
	}
}