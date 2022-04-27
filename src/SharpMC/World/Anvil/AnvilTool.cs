namespace SharpMC.World.Anvil
{
    internal static class AnvilTool
    {
        internal static byte Nibble4(byte[] arr, int index)
        {
            return (byte) (index % 2 == 0 ? arr[index / 2] & 0x0F : (arr[index / 2] >> 4) & 0x0F);
        }

        internal static void SetNibble4(byte[] arr, int index, byte value)
        {
            if (index % 2 == 0)
            {
                arr[index / 2] = (byte) ((value & 0x0F) | arr[index / 2]);
            }
            else
            {
                arr[index / 2] = (byte) (((value << 4) & 0xF0) | arr[index / 2]);
            }
        }
    }
}