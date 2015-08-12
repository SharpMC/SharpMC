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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zlib;
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
		private byte[] GetVarIntBytes(int integer)
		{
			List<Byte> bytes = new List<byte>();
			while ((integer & -128) != 0)
			{
				bytes.Add((byte)(integer & 127 | 128));
				integer = (int)(((uint)integer) >> 7);
			}
			bytes.Add((byte)integer);
			return bytes.ToArray();
		}

		private bool ReadUncompressed(ClientWrapper client, NetworkStream clientStream, int dlength)
		{
			var buffie = new byte[dlength];
			int receivedData;
			receivedData = clientStream.Read(buffie, 0, buffie.Length);
			if (receivedData > 0)
			{
				var buf = new DataBuffer(client);

				if (client.Decrypter != null)
				{
					var date = new byte[4096];
					client.Decrypter.TransformBlock(buffie, 0, buffie.Length, date, 0);
					buf.BufferedData = date;
				}
				else
				{
					buf.BufferedData = buffie;
				}

				buf.BufferedData = buffie;

				buf.Size = dlength;
				var packid = buf.ReadVarInt();

				if (!new PackageFactory(client, buf).Handle(packid))
				{
					ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
				}

				buf.Dispose();
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ReadCompressed(ClientWrapper client, NetworkStream clientStream, int dlength)
		{
			var buffie = new byte[dlength];
			int receivedData;
			receivedData = clientStream.Read(buffie, 0, buffie.Length);
			buffie = ZlibStream.UncompressBuffer(buffie);

			if (receivedData > 0)
			{
				var buf = new DataBuffer(client);

				if (client.Decrypter != null)
				{
					var date = new byte[4096];
					client.Decrypter.TransformBlock(buffie, 0, buffie.Length, date, 0);
					buf.BufferedData = date;
				}
				else
				{
					buf.BufferedData = buffie;
				}

				buf.BufferedData = buffie;

				buf.Size = dlength;
				var packid = buf.ReadVarInt();

				if (!new PackageFactory(client, buf).Handle(packid))
				{
					ConsoleFunctions.WriteWarningLine("Unknown packet received! \"0x" + packid.ToString("X2") + "\"");
				}

				buf.Dispose();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void HandleClientCommNew(object client)
		{
			var tcpClient = (TcpClient) client;
			var clientStream = tcpClient.GetStream();
			var Client = new ClientWrapper(tcpClient);

			Globals.ClientManager.AddClient(ref Client);

			while (true)
			{
				try
				{
					while (!clientStream.DataAvailable)
					{
						if (Client.Kicked)
						{
							break;
						}
						Thread.Sleep(5);
					}

					if (Client.Kicked)
					{
						break;
					}

					if (ServerSettings.UseCompression && Client.PacketMode == PacketMode.Play)
					{
						int packetLength = ReadVarInt(clientStream);
						int dataLength = ReadVarInt(clientStream);
						int actualDataLength = packetLength - GetVarIntBytes(dataLength).Length;
						
						if (dataLength == 0)
						{
							if (!ReadCompressed(Client, clientStream, actualDataLength)) break;
						}
						else
						{
							if (!ReadUncompressed(Client, clientStream, dataLength)) break;
						}
					}
					else
					{
						int dlength = ReadVarInt(clientStream);
						if (!ReadUncompressed(Client, clientStream, dlength)) break;
					}
				}
				catch (Exception ex)
				{
					//Exception, disconnect!
					ConsoleFunctions.WriteDebugLine("Error: \n" + ex);
					if (ServerSettings.ReportExceptionsToClient)
					{
						new Disconnect(Client)
						{
							Reason = new McChatMessage("§fServer threw an exception!\n\nFor the nerdy people: \n" + ex.Message)
						}.Write();
					}
					else
					{
						new Disconnect(Client)
						{
							Reason = new McChatMessage("§fYou were kicked because of an unknown problem!")
						}.Write();
					}
					break;
				}
			}

			if (Client.Kicked)
			{
				new Disconnect(Client)
				{
					Reason = new McChatMessage("§fYou were kicked because of a network problem!")
				}.Write();
			}
			
			//Close the connection with the client. :)
			Client.ThreadPool.KillAllThreads();
			//Client.StopKeepAliveTimer();

			if (Client.Player != null)
			{
				Client.Player.SavePlayer();
				Client.Player.Level.RemovePlayer(Client.Player.EntityId);
				Client.Player.Level.BroadcastPlayerRemoval(Client);
			}

			Client.TcpClient.Close();
			Globals.ClientManager.RemoveClient(Client);
			Thread.CurrentThread.Abort();
		}
	}
}