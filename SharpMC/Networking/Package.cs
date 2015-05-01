using System.Net.Sockets;
using SharpMC.Utils;

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