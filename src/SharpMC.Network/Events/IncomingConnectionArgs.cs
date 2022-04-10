using System;

namespace SharpMC.Network.Events
{
    public sealed class IncomingConnectionArgs : EventArgs
    {
        internal IncomingConnection Connection { get; }
        public bool ConnectionAccepted { get; private set; }

        internal IncomingConnectionArgs(IncomingConnection connection)
        {
            Connection = connection;
            ConnectionAccepted = false;
        }

        public void Allow()
        {
            ConnectionAccepted = true;
        }

        public void Deny()
        {
            ConnectionAccepted = false;
        }
    }
}