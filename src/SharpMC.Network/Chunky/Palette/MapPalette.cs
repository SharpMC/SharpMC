using System;
using System.Collections.Generic;
using System.Linq;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Chunky.Utils;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class MapPalette : IPalette, IEquatable<MapPalette>
    {
        public int MaxId { get; }
        public int[] IdsToState { get; }
        public IDictionary<int, int> StatesToId { get; } = new SortedDictionary<int, int>();
        public int NextId { get; set; }

        public MapPalette(int bitsPerEntry)
        {
            MaxId = (1 << bitsPerEntry) - 1;
            IdsToState = new int[MaxId + 1];
        }

        public MapPalette(int bitsPerEntry, IMinecraftReader input)
            : this(bitsPerEntry)
        {
            var paletteLength = input.ReadVarInt();
            for (var i = 0; i < paletteLength; i++)
            {
                var state = input.ReadVarInt();
                IdsToState[i] = state;
                if (!StatesToId.ContainsKey(state))
                    StatesToId[state] = i;
            }
            NextId = paletteLength;
        }

        public int Size => NextId;

        public int StateToId(int state)
        {
            var id = StatesToId.TryGetValue(state, out var value)
                ? value
                : default(int?);
            if (id == null && Size < MaxId + 1)
            {
                id = NextId++;
                IdsToState[id.Value] = state;
                StatesToId[state] = id.Value;
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
                return IdsToState[id];
            }
            return 0;
        }

        #region Hashcode

        public bool Equals(MapPalette other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MaxId == other.MaxId && Equals(IdsToState, other.IdsToState) &&
                   Equals(StatesToId, other.StatesToId) && NextId == other.NextId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MapPalette) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(MaxId, IdsToState, StatesToId, NextId);

        public static bool operator ==(MapPalette left, MapPalette right) 
            => Equals(left, right);

        public static bool operator !=(MapPalette left, MapPalette right) 
            => !Equals(left, right);

        #endregion
    }
}