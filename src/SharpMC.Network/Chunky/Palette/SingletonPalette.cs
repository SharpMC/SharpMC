using System;
using SharpMC.Network.Chunky.Palette;
using SharpMC.Network.Util;

namespace SharpMC.Chunky.Palette
{
    public class SingletonPalette : IPalette, IEquatable<SingletonPalette>
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

        #region Hashcode

        public bool Equals(SingletonPalette other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return State == other.State;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SingletonPalette) obj);
        }

        public override int GetHashCode() => State;

        public static bool operator ==(SingletonPalette left, SingletonPalette right)
            => Equals(left, right);

        public static bool operator !=(SingletonPalette left, SingletonPalette right)
            => !Equals(left, right);

        #endregion
    }
}