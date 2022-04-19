using System;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class ListPalette : IPalette, IEquatable<ListPalette>
    {
        public int MaxId { get; }
        public int[] Data { get; }
        public int NextId { get; set; }

        public ListPalette(int bitsPerEntry)
        {
            BitsPerEntry = bitsPerEntry;

            MaxId = (1 << bitsPerEntry) - 1;
            Data = new int[MaxId + 1];
        }

        public ListPalette(int bitsPerEntry, IMinecraftReader input)
            : this(bitsPerEntry)
        {
            var paletteLength = input.ReadVarInt();
            for (var i = 0; i < paletteLength; i++)
            {
                Data[i] = input.ReadVarInt();
            }
            NextId = paletteLength;
        }

        public int Size => NextId;

        public int StateToId(int state)
        {
            var id = -1;
            for (var i = 0; i < NextId; i++)
            {
                if (Data[i] == state)
                {
                    id = i;
                    break;
                }
            }
            if (id == -1 && Size < MaxId + 1)
            {
                id = NextId++;
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

        public int BitsPerEntry { get; }

        #region Hashcode

        public bool Equals(ListPalette other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MaxId == other.MaxId && Equals(Data, other.Data) && NextId == other.NextId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ListPalette) obj);
        }

        public override int GetHashCode() 
            => HashCode.Combine(MaxId, Data, NextId);

        public static bool operator ==(ListPalette left, ListPalette right) 
            => Equals(left, right);

        public static bool operator !=(ListPalette left, ListPalette right) 
            => !Equals(left, right);

        #endregion
    }
}