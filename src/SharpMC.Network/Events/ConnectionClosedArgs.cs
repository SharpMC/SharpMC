using System;

namespace SharpMC.Network.Events
{
    public class ConnectionClosedArgs : EventArgs
    {
        public NetConnection Connection { get; }
        public bool Graceful { get; }

        internal ConnectionClosedArgs(NetConnection connection, bool requested)
        {
            Connection = connection;
            Graceful = requested;
        }
    }
}