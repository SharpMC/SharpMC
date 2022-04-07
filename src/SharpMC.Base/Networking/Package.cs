using System.Net.Sockets;
using SharpMC.Core.Utils;
using SharpMC.World;

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