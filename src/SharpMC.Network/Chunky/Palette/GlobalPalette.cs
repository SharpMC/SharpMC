using SharpMC.Network.Chunky.Palette;

namespace SharpMC.Chunky.Palette
{
    public class GlobalPalette : IPalette
    {
        public int Size => int.MaxValue;

        public int StateToId(int state)
        {
            return state;
        }

        public int IdToState(int id)
        {
            return id;
        }

        public int BitsPerEntry => 0;
    }
}