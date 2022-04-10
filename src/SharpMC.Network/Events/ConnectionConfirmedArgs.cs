using System;

namespace SharpMC.Network.Events
{
    public class ConnectionConfirmedArgs : EventArgs
    {
        public NetConnection Connection { get; }

        internal ConnectionConfirmedArgs(NetConnection connection)
        {
            Connection = connection;
        }
    }
}