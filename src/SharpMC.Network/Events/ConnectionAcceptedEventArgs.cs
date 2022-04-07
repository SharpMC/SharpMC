using System;

namespace SharpMC.Network.Events
{
    public class ConnectionAcceptedEventArgs : EventArgs
    {
        public NetConnection Connection { get; }

        internal ConnectionAcceptedEventArgs(NetConnection connection)
        {
            Connection = connection;
        }
    }
}