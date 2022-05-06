using System;

namespace SharpMC.Network.Core
{
    internal readonly record struct SendData
    {
        public byte[] Buffer { get; init; }
        public DateTime Time { get; init; }

        public SendData(byte[] buffer)
        {
            Buffer = buffer;
            Time = DateTime.UtcNow;
        }
    }
}