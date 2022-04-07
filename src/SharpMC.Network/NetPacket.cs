namespace SharpMC.Network
{
    public class NetPacket
    {
        public byte[] Buffer { get; }

        internal NetPacket(byte[] data)
        {
            Buffer = data;
        }
    }
}