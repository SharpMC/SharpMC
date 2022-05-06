using System;

namespace SharpMC.Network.Events
{
    public class ConnectionCreatedArgs : EventArgs
    {
        public NetConnection Connection { get; }

        internal ConnectionCreatedArgs(NetConnection connection)
        {
            Connection = connection;
        }
    }
}