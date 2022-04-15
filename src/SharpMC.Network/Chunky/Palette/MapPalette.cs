using System.Collections.Generic;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Chunky.Utils;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class MapPalette : IPalette
    {
        private readonly Dictionary<int, int?> _stateToId = new Dictionary<int, int?>();
        private int _nextId;

        private readonly int _maxId;
        private readonly int[] _idToState;

        public MapPalette(int bitsPerEntry)
        {
            _maxId = (1 << bitsPerEntry) - 1;
            _idToState = new int[_maxId + 1];
        }

        public MapPalette(int bitsPerEntry, IMinecraftReader input)
            : this(bitsPerEntry)
        {
            var paletteLength = input.ReadVarInt();
            for (var i = 0; i < paletteLength; i++)
            {
                var state = input.ReadVarInt();
                _idToState[i] = state;
                _stateToId.PutIfAbsent(state, i);
            }
            _nextId = paletteLength;
        }

        public int Size => _nextId;

        public int StateToId(int state)
        {
            var id = _stateToId.GetOrDefault(state);
            if (id == null && Size < _maxId + 1)
            {
                id = _nextId++;
                _idToState[id.Value] = state;
                _stateToId[state] = id;
            }
            if (id != null)
            {
                return id.Value;
            }
            return -1;
        }

        public int IdToState(int id)
        {
            if (id >= 0 && id < Size)
            {
                return _idToState[id];
            }
            return 0;
        }
    }
}