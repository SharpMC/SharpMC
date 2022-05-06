using System;

namespace SharpMC.Network.Events
{
    public class ConnectionAcceptedArgs : EventArgs
    {
        public NetConnection Connection { get; }
        
        internal ConnectionAcceptedArgs(NetConnection connection)
        {
            Connection = connection;
        }
    }
}