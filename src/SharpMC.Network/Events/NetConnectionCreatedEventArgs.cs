using System;

namespace SharpMC.Network.Events
{
	public class NetConnectionCreatedEventArgs : EventArgs
	{
		public NetConnection Connection { get; }

		internal NetConnectionCreatedEventArgs(NetConnection connection)
		{
			Connection = connection;
		}
	}
}