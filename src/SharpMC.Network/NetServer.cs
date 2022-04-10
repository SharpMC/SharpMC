using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using SharpMC.Logging;
using SharpMC.Network.Events;

namespace SharpMC.Network
{
    public class NetServer
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(NetServer));
        
		public NetConnectionFactory NetConnectionFactory { get; set; }

		private CancellationTokenSource CancellationToken { get; set; }
        private ConcurrentDictionary<EndPoint, NetConnection> Connections { get; set; }

		internal NetConfiguration Configuration { get; }
        private Socket ListenerSocket { get; set; }
        
        public NetServer(NetConfiguration configuration)
        {
            Configuration = configuration;
            SetDefaults();
            Configure();
        }

        public EventHandler<ConnectionAcceptedArgs> OnConnectionAccepted;

        private void SetDefaults()
        {
            CancellationToken = new CancellationTokenSource();

            Connections = new ConcurrentDictionary<EndPoint, NetConnection>();
			NetConnectionFactory = new NetConnectionFactory();
        }

        private void Configure()
        {
            if (Configuration.Protocol == ProtocolType.Tcp)
            {
                ListenerSocket = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      System.Net.Sockets.ProtocolType.Tcp);
            }
            else
            {
                throw new NotSupportedException("This protocol is currently not supported!");
            }
        }

        public void Start()
        {
            if (Configuration.Protocol == ProtocolType.Tcp)
            {
                ListenerSocket.Bind(new IPEndPoint(Configuration.Host, Configuration.Port));
                ListenerSocket.Listen(10);

                ListenerSocket.BeginAccept(ConnectionCallback, null);
            }
        }

        public void Stop()
        {
            CancellationToken.Cancel();

            foreach (var i in Connections)
            {
                i.Value.Stop();
            }
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            Socket socket = null;
            try
            {
                socket = ListenerSocket.EndAccept(ar);
            }
            catch
            {
                Log.LogWarning("Failed to accept connection!");
            }

            ListenerSocket.BeginAccept(ConnectionCallback, null);

            if (socket == null) return;
			
			var conn = NetConnectionFactory.CreateConnection(Direction.Client, socket, ConfirmedAction);

			if (Connections.TryAdd(socket.RemoteEndPoint, conn))
			{
				conn.OnConnectionClosed += (sender, args) =>
				{
					if (Connections.TryRemove(args.Connection.RemoteEndPoint, out var nc))
					{
						Log.LogInformation("Client disconnected!");
					}
				};
				conn.Initialize();
			}
			else
			{
				Log.LogWarning("Could not create new active connection!");
			}
		}

        private void ConfirmedAction(NetConnection conn)
        {
            OnConnectionAccepted?.Invoke(this, new ConnectionAcceptedEventArgs(conn));
        }
    }
}