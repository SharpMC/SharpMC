#region Imports

using System;

#endregion

namespace SharpMC.Network.Events
{
    public sealed class ConnectionClosedEventArgs : EventArgs
    {
        public NetConnection Connection { get; }
        public bool Graceful { get; }
        internal ConnectionClosedEventArgs(NetConnection connection, bool requested)
        {
            Connection = connection;
            Graceful = requested;
        }
    }
}
