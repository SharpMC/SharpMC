using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class ListPalette : IPalette
    {
        public int BitsPerEntry { get; }

        private readonly int _maxId;
        public int[] Data { get; }

        public ListPalette(int bitsPerEntry)
        {
            BitsPerEntry = bitsPerEntry;

            _maxId = (1 << bitsPerEntry) - 1;
            Data = new int[_maxId + 1];
        }

        public ListPalette(int bitsPerEntry, IMinecraftReader input)
            : this(bitsPerEntry)
        {
            var paletteLength = input.ReadVarInt();
            for (var i = 0; i < paletteLength; i++)
            {
                Data[i] = input.ReadVarInt();
            }
            Size = paletteLength;
        }

        public int Size { get; private set; }

        public int StateToId(int state)
        {
            var id = -1;
            for (var i = 0; i < Size; i++)
            {
                if (Data[i] == state)
                {
                    id = i;
                    break;
                }
            }
            if (id == -1 && Size < _maxId + 1)
            {
                id = Size++;
                Data[id] = state;
            }
            return id;
        }

        public int IdToState(int id)
        {
            if (id >= 0 && id < Size)
            {
                return Data[id];
            }
            return 0;
        }
    }
}