// Distributed under the MIT license
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
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SharpMC.Core.Networking.Packages;

namespace SharpMC.Core.Utils
{
	public enum PacketMode
	{
		Ping,
		Status,
		Login,
		Play
	}

	public class ClientWrapper
	{
		private readonly Queue<byte[]> _commands = new Queue<byte[]>();
		private readonly AutoResetEvent _resume = new AutoResetEvent(false);
		internal bool EncryptionEnabled = false;
		public PacketMode PacketMode = PacketMode.Ping;
		public Player Player;
		public TcpClient TcpClient;
		public MyThreadPool ThreadPool;
		internal int Protocol = 0;
		internal int ClientIdentifier = -1;
		public bool Kicked = false;
		public bool SetCompressionSend = false;

		public ClientWrapper(TcpClient client)
		{
			TcpClient = client;
			if (client != null)
			{
				ThreadPool = new MyThreadPool();
				ThreadPool.LaunchThread(ThreadRun);

				var bytes = new byte[8];
				Globals.Rand.NextBytes(bytes);
				ConnectionId = Encoding.ASCII.GetString(bytes).Replace("-", "");
			}
		}

		internal byte[] SharedKey { get; set; }
		internal ICryptoTransform Encrypter { get; set; }
		internal ICryptoTransform Decrypter { get; set; }
		internal string ConnectionId { get; set; }
		internal string Username { get; set; }

		public void AddToQuee(byte[] data, bool quee = false)
		{
			if (TcpClient != null)
			{
				if (quee)
				{
					lock (_commands)
					{
						_commands.Enqueue(data);
					}
					_resume.Set();
				}
				else
				{
					SendData(data);
				}
			}
		}

		private void ThreadRun()
		{
			while (_resume.WaitOne())
			{
				byte[] command;
				lock (_commands)
				{
					command = _commands.Dequeue();
				}
				SendData(command);
			}
		}

		public void SendData(byte[] data)
		{
			if (TcpClient != null)
			{
				try
				{
					if (Encrypter != null)
					{
						var toEncrypt = data;
						data = new byte[toEncrypt.Length];
						Encrypter.TransformBlock(toEncrypt, 0, toEncrypt.Length, data, 0);

						var a = TcpClient.GetStream();
						a.Write(data, 0, data.Length);
						a.Flush();
					}
					else
					{
						//var a = TcpClient.GetStream();
						//a.Write(data, 0, data.Length);
						//a.Flush();
						TcpClient.Client.Send(data);
					}
					Globals.ClientManager.CleanErrors(this);
				}
				catch(Exception ex)
				{
					//ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
					Globals.ClientManager.PacketError(this, ex);
				}
			}
		}

		public long GetLastPing
		{
			get { return lastPing; }
		}
		private long lastPing = 0;
		public void UpdatePing()
		{
			var time = UnixTimeNow();
			var ping = time - lastPing;
			lastPing = time;
			Globals.ClientManager.ReportPing(this);

			var packet = new PlayerListItem(null)
			{
				Action = 2,
				Latency = (int)ping,
				Uuid = Player.Uuid
			};
			Globals.BroadcastPacket(packet);
		}

		private long UnixTimeNow()
		{
			var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalSeconds;
		}
	}
}