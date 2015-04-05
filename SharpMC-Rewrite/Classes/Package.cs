using System.Diagnostics;
using System.Net.Sockets;

namespace SharpMCRewrite.Classes
{
	public abstract class Package
	{
		private NetworkStream _stream;
		private Stopwatch _timer = new Stopwatch();
		public MSGBuffer Buffer;
		public ClientWrapper Client;
		public int ReadId;
		public int SendId;

		public Package(ClientWrapper client)
		{
			if (!client.TcpClient.Connected) return;
			Client = client;
			_stream = client.TcpClient.GetStream();
			Buffer = new MSGBuffer(client);
		}

		public Package(ClientWrapper client, MSGBuffer buffer)
		{
			if (!client.TcpClient.Connected) return;
			Client = client;
			_stream = client.TcpClient.GetStream();
			Buffer = buffer;
		}

		public virtual void Read()
		{
		}

		public virtual void Write()
		{
		}

		public void Broadcast(bool self = true, Player source = null)
		{
			foreach (var i in Globals.Level.OnlinePlayers)
			{
				if (!self && i == source)
				{
					continue;
				}
				Client = i.Wrapper;
				Buffer = new MSGBuffer(i.Wrapper);
				_stream = i.Wrapper.TcpClient.GetStream();
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