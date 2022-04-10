using System;

namespace SharpMC.Network
{
    internal struct ReceivedData
    {
        public byte[] OldBuffer;
        public byte[] Buffer;
        public DateTime Time;

        public ReceivedData(byte[] buffer, byte[] oldBuffer)
        {
            Buffer = buffer;
            OldBuffer = oldBuffer;
            Time = DateTime.UtcNow;
        }
    }
}