using System;
using SharpMC.Network.Packets;

namespace SharpMC.Network.Events
{
    public class PacketReceivedArgs : EventArgs
    {
        public Packet Packet { get; }
        internal bool IsInvalid { get; set; } = false;

        internal PacketReceivedArgs(Packet netPacket)
        {
            Packet = netPacket;
        }

        public void Invalid()
        {
            IsInvalid = true;
        }
    }
}