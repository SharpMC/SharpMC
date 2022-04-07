﻿using System;

namespace SharpMC.Network.Events
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public Packets.Packet Packet { get; }
        internal bool IsInvalid { get; set; } = false;

        internal PacketReceivedEventArgs(Packets.Packet netPacket)
        {
            Packet = netPacket;
        }

        public void Invalid()
        {
            IsInvalid = true;
        }
    }
}