using System;
using SharpMC.Network.Chunky.Palette;

namespace SharpMC.Chunky.Palette
{
    public class GlobalPalette : IPalette, IEquatable<GlobalPalette>
    {
        public int Size => 2147483647;

        public int StateToId(int state)
        {
            return state;
        }

        public int IdToState(int id)
        {
            return id;
        }

        public int BitsPerEntry => 0;

        #region Hashcode

        public bool Equals(GlobalPalette other) => true;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GlobalPalette) obj);
        }

        public override int GetHashCode() => 1;

        public static bool operator ==(GlobalPalette left, GlobalPalette right)
            => Equals(left, right);

        public static bool operator !=(GlobalPalette left, GlobalPalette right)
            => !Equals(left, right);

        #endregion
    }
}