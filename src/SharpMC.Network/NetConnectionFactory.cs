using System;
using System.Net.Sockets;
using SharpMC.Network.Events;

namespace SharpMC.Network
{
	public class NetConnectionFactory
	{
		public EventHandler<NetConnectionCreatedEventArgs> OnConnectionCreated;

		internal NetConnection CreateConnection(Direction direction, Socket socket, ConnectionConfirmed confirmedAction = null)
		{
			var connection = Create(direction, socket, confirmedAction);

			if (connection == null) return null;

			OnConnectionCreated?.Invoke(null, new NetConnectionCreatedEventArgs(connection));
			return connection;
		}

		protected virtual NetConnection Create(Direction direction, Socket socket, ConnectionConfirmed confirmedAction = null)
		{
			var conn = new NetConnection(direction, socket, confirmedAction);
			return conn;
		}
	}
}