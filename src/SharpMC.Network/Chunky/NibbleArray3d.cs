using System;
using SharpMC.Network.Chunky.Utils;
using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class NibbleArray3d : IEquatable<NibbleArray3d>
    {
        public byte[] Data { get; }

        public NibbleArray3d(byte[] data)
        {
            Data = data ?? throw new ArgumentException("data is marked non-null but is null");
        }

        public NibbleArray3d(int size)
            : this(new byte[size >> 1])
        {
        }

        public NibbleArray3d(IMinecraftReader input, int size)
            : this(input.Read(size))
        {
            if (input == null)
                throw new ArgumentException("in is marked non-null but is null");
        }

        public void Write(IMinecraftWriter output)
        {
            if (output == null)
                throw new ArgumentException("out is marked non-null but is null");
            output.Write(Data);
        }

        public int this[(int x, int y, int z) index]
        {
            get
            {
                var (x, y, z) = index;
                var key = y << 8 | z << 4 | x;
                var myIndex = key >> 1;
                var part = key & 0x1;
                return part == 0 ? Data[myIndex] & 0xF : Data[myIndex] >> 4 & 0xF;
            }
            set
            {
                var (x, y, z) = index;
                var key = y << 8 | z << 4 | x;
                var myIndex = key >> 1;
                var part = key & 0x1;
                if (part == 0)
                {
                    Data[myIndex] = (byte) (Data[myIndex] & 0xF0 | value & 0xF);
                }
                else
                {
                    Data[myIndex] = (byte) (Data[myIndex] & 0xF | (value & 0xF) << 4);
                }
            }
        }

        public void Fill(int val)
        {
            for (var index = 0; index < Data.Length << 1; index++)
            {
                var ind = index >> 1;
                var part = index & 0x1;
                if (part == 0)
                {
                    Data[ind] = (byte) (Data[ind] & 0xF0 | val & 0xF);
                }
                else
                {
                    Data[ind] = (byte) (Data[ind] & 0xF | (val & 0xF) << 4);
                }
            }
        }

        public override string ToString()
        {
            return $"NibbleArray3d(data={Data.ToArrayString()})";
        }

        #region Hashcode

        public bool Equals(NibbleArray3d other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Data.ArrayEquals(other.Data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NibbleArray3d) obj);
        }

        public override int GetHashCode()
            => Data != null ? Data.GetHashCode() : 0;

        public static bool operator ==(NibbleArray3d left, NibbleArray3d right)
            => Equals(left, right);

        public static bool operator !=(NibbleArray3d left, NibbleArray3d right)
            => !Equals(left, right);

        #endregion
    }
}