namespace SharpMC.Network.Core
{
    public readonly record struct NetPacket
    {
        public byte[] Buffer { get; init; }

        internal NetPacket(byte[] data)
        {
            Buffer = data;
        }
    }
}