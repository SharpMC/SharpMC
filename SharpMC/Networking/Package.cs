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

using System.Net.Sockets;
using SharpMC.Entity;
using SharpMC.Utils;
using SharpMC.Worlds;

namespace SharpMC.Networking
{
	public abstract class Package
	{
		protected Package(ClientWrapper client)
		{
			if (!client.TcpClient.Connected) return;
			Client = client;
			Stream = client.TcpClient.GetStream();
			Buffer = new MSGBuffer(client);
		}

		protected Package(ClientWrapper client, MSGBuffer buffer)
		{
			if (!client.TcpClient.Connected) return;
			Client = client;
			Stream = client.TcpClient.GetStream();
			Buffer = buffer;
		}

		public NetworkStream Stream { get; private set; }
		public MSGBuffer Buffer { get; private set; }
		public ClientWrapper Client { get; private set; }
		public int ReadId { get; set; }
		public int SendId { get; set; }

		public virtual void Read()
		{
		}

		public virtual void Write()
		{
		}

		public void Broadcast(Level level, bool self = true, Player source = null)
		{
			foreach (var i in level.OnlinePlayers)
			{
				if (!self && i == source)
				{
					continue;
				}
				Client = i.Wrapper;
				Buffer = new MSGBuffer(i.Wrapper);
				Stream = i.Wrapper.TcpClient.GetStream();
				Write();
			}
		}
	}

	public abstract class Package<T> : Package where T : Package<T>
	{
		protected Package(ClientWrapper client) : base(client)
		{
		}

		protected Package(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
		}
	}
}