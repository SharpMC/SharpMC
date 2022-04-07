using System;

namespace SharpMC.Network
{
    internal struct SendData
    {
        public byte[] Buffer;
        public DateTime Time;

        public SendData(byte[] buffer)
        {
            Buffer = buffer;
            Time = DateTime.UtcNow;
        }
    }
}