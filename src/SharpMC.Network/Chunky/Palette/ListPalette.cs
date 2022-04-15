using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class ListPalette : IPalette
    {
        private readonly int _maxId;
        private readonly int[] _data;

        public ListPalette(int bitsPerEntry)
        {
            _maxId = (1 << bitsPerEntry) - 1;
            _data = new int[_maxId + 1];
        }

        public ListPalette(int bitsPerEntry, IMinecraftReader input)
            : this(bitsPerEntry)
        {
            var paletteLength = input.ReadVarInt();
            for (var i = 0; i < paletteLength; i++)
            {
                _data[i] = input.ReadVarInt();
            }
            Size = paletteLength;
        }

        public int Size { get; private set; }

        public int StateToId(int state)
        {
            var id = -1;
            for (var i = 0; i < Size; i++)
            {
                if (_data[i] == state)
                {
                    id = i;
                    break;
                }
            }
            if (id == -1 && Size < _maxId + 1)
            {
                id = Size++;
                _data[id] = state;
            }
            return id;
        }

        public int IdToState(int id)
        {
            if (id >= 0 && id < Size)
            {
                return _data[id];
            }
            return 0;
        }
    }
}