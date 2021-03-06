﻿// Distrubuted under the MIT license
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

using System.Net.Sockets;
using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;

namespace SharpMC.Core.Networking
{
	public abstract class Package
	{
		protected Package(ClientWrapper client)
		{
			if (client == null) return;
			if (!client.TcpClient.Connected) return;
			Client = client;
			Stream = client.TcpClient.GetStream();
			Buffer = new DataBuffer(client);
		}

		protected Package(ClientWrapper client, DataBuffer buffer)
		{
			if (client == null) return;
			if (!client.TcpClient.Connected) return;
			Client = client;
			Stream = client.TcpClient.GetStream();
			Buffer = buffer;
		}

		public NetworkStream Stream { get; private set; }
		public DataBuffer Buffer { get; private set; }
		public ClientWrapper Client { get; private set; }
		public int ReadId { get; set; }
		public int SendId { get; set; }

		internal void SetTarget(ClientWrapper client)
		{
			Client = client;
			Stream = client.TcpClient.GetStream();
			Buffer = new DataBuffer(client);
		}

		public virtual void Read()
		{
		}

		public virtual void Write()
		{
		}

		public void Broadcast(Level level, bool self = true, Player source = null)
		{
			foreach (var i in level.GetOnlinePlayers)
			{
				try
				{
					if (i != null && i.Wrapper != null & i.Wrapper.TcpClient != null)
					{
						if (!self && i == source)
						{
							continue;
						}
						Client = i.Wrapper;
						Buffer = new DataBuffer(i.Wrapper);
						Stream = i.Wrapper.TcpClient.GetStream(); //Exception here. (sometimes)
						Write();
					}
				}
				catch
				{
					//Catch any exception just to be sure the broadcast works.
					//TODO: Fix the exception.
				}
			}
		}
	}

	public abstract class Package<T> : Package where T : Package<T>
	{
		protected Package(ClientWrapper client) : base(client)
		{
		}

		protected Package(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
		}
	}
}