using System;
using static SharpMC.Network.Chunky.Utils.Constants;

namespace SharpMC.Chunky
{
    public class BitStorage : IEquatable<BitStorage>
    {
        public long[] Data { get; }
        public int BitsPerEntry { get; }
        public int Size { get; }
        public long MaxValue { get; }
        public int ValuesPerLong { get; }
        public long DivideMultiply { get; }
        public long DivideAdd { get; }
        public int DivideShift { get; }

        public BitStorage(int bitsPerEntry, int size, long[] data = null)
        {
            if (bitsPerEntry < 1 || bitsPerEntry > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerEntry), bitsPerEntry,
                    "bits per entry must be between 1 and 32 (inclusive)");
            }
            BitsPerEntry = bitsPerEntry;
            Size = size;
            MaxValue = (1L << bitsPerEntry) - 1L;
            ValuesPerLong = (char) (64 / bitsPerEntry);
            var expectedLength = (size + ValuesPerLong - 1) / ValuesPerLong;
            if (data != null)
            {
                if (data.Length != expectedLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), data.Length,
                        $"Expected {expectedLength} longs but got {data.Length} longs");
                }
                Data = data;
            }
            else
            {
                Data = new long[expectedLength];
            }
            var magicIndex = 3 * (ValuesPerLong - 1);
            DivideMultiply = MagicValuesLong[magicIndex];
            DivideAdd = MagicValuesLong[magicIndex + 1];
            DivideShift = MagicValues[magicIndex + 2];
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index > Size - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        index, "Index out of bounds!");
                }
                var cellIndex = GetCellIndex(index);
                var bitIndex = GetBitIndex(index, cellIndex);
                return (int) (Data[cellIndex] >> bitIndex & MaxValue);
            }
            set
            {
                if (index < 0 || index > Size - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        index, "Index out of bounds!");
                }
                if (value < 0 || value > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        value, "Value cannot be outside of accepted range.");
                }
                var cellIndex = GetCellIndex(index);
                var bitIndex = GetBitIndex(index, cellIndex);
                Data[cellIndex] = Data[cellIndex] & ~(MaxValue << bitIndex)
                                  | (value & MaxValue) << bitIndex;
            }
        }

        public int[] ToArray()
        {
            var result = new int[Size];
            var index = 0;
            foreach (var rawCell in Data)
            {
                var cell = rawCell;
                for (var bitIndex = 0; bitIndex < ValuesPerLong; bitIndex++)
                {
                    result[index++] = (int) (cell & MaxValue);
                    cell >>= BitsPerEntry;
                    if (index >= Size)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private int GetCellIndex(int index)
        {
            return (int) (index * DivideMultiply + DivideAdd >> 32 >> DivideShift);
        }

        private int GetBitIndex(int index, int cellIndex)
        {
            return (index - cellIndex * ValuesPerLong) * BitsPerEntry;
        }

        #region Hashcode

        public bool Equals(BitStorage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Data, other.Data) && BitsPerEntry == other.BitsPerEntry &&
                   Size == other.Size && MaxValue == other.MaxValue &&
                   ValuesPerLong == other.ValuesPerLong &&
                   DivideMultiply == other.DivideMultiply &&
                   DivideAdd == other.DivideAdd && DivideShift == other.DivideShift;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BitStorage) obj);
        }

        public override int GetHashCode()
            => HashCode.Combine(Data, BitsPerEntry, Size, MaxValue, ValuesPerLong,
                DivideMultiply, DivideAdd, DivideShift);

        public static bool operator ==(BitStorage left, BitStorage right)
            => Equals(left, right);

        public static bool operator !=(BitStorage left, BitStorage right)
            => !Equals(left, right);

        #endregion
    }
}