using System;

namespace SharpMC.Network.Core
{
    internal readonly record struct ReceivedData
    {
        public byte[] OldBuffer { get; init; }
        public byte[] Buffer { get; init; }
        public DateTime Time { get; init; }

        public ReceivedData(byte[] buffer, byte[] oldBuffer)
        {
            Buffer = buffer;
            OldBuffer = oldBuffer;
            Time = DateTime.UtcNow;
        }
    }
}