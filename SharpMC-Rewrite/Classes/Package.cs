using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace SharpMCRewrite.NET
{
	public abstract partial class Package
	{
		public int SendId;
		public int ReadId;
		private NetworkStream _stream;
		public ClientWrapper Client;
		public MSGBuffer Buffer;
		private Stopwatch _timer = new Stopwatch();

		public Package(ClientWrapper client)
		{
			Client = client;
			_stream = client.TCPClient.GetStream();
			Buffer = new MSGBuffer(client);
		}

		public Package(ClientWrapper client, MSGBuffer buffer)
		{
			Client = client;
			_stream = client.TCPClient.GetStream();
			Buffer = buffer;
		}

		public virtual void Read()
		{
			
		}

		public virtual void Write()
		{
			
		}
	}

	public abstract partial class Package<T> : Package where T : Package<T>
	{
		protected Package(ClientWrapper client) : base(client)
		{
		}

		protected Package(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
		}
	}
}
