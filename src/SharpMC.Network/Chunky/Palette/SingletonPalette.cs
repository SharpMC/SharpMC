using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class SingletonPalette : IPalette
    {
        public int State { get; }

        public SingletonPalette(int state)
        {
            State = state;
        }

        public SingletonPalette(IMinecraftReader input)
        {
            State = input.ReadVarInt();
        }

        public int Size => 1;

        public int StateToId(int state)
        {
            return State == state ? 0 : -1;
        }

        public int IdToState(int id)
        {
            return id == 0 ? State : 0;
        }

        public int BitsPerEntry => 0;
    }
}