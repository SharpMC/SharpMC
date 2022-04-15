using System;
using static SharpMC.Network.Chunky.Utils.Constants;

namespace SharpMC.Chunky
{
    public class BitStorage
    {
        public long[] Data { get; }
        public int BitsPerEntry { get; }
        private readonly int _size;

        private readonly long _maxValue;
        private readonly int _valuesPerLong;
        private readonly long _divideMultiply;
        private readonly long _divideAdd;
        private readonly int _divideShift;

        public BitStorage(int bitsPerEntry, int size)
            : this(bitsPerEntry, size, null)
        {
        }

        public BitStorage(int bitsPerEntry, int size, long[] data)
        {
            if (bitsPerEntry < 1 || bitsPerEntry > 32)
            {
                throw new ArgumentException("bitsPerEntry must be between 1 and 32, inclusive.");
            }
            BitsPerEntry = bitsPerEntry;
            _size = size;
            _maxValue = (1L << bitsPerEntry) - 1L;
            _valuesPerLong = (char) (64 / bitsPerEntry);
            var expectedLength = (size + _valuesPerLong - 1) / _valuesPerLong;
            if (data != null)
            {
                if (data.Length != expectedLength)
                {
                    throw new ArgumentException($"Expected {expectedLength} longs but got {data.Length} longs");
                }
                Data = data;
            }
            else
            {
                Data = new long[expectedLength];
            }
            var magicIndex = 3 * (_valuesPerLong - 1);
            _divideMultiply = MagicValues[magicIndex];
            _divideAdd = MagicValues[magicIndex + 1];
            _divideShift = MagicValues[magicIndex + 2];
        }

        public int Get(int index)
        {
            if (index < 0 || index > _size - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            var cellIndex = CellIndex(index);
            var bitIndex = BitIndex(index, cellIndex);
            return (int) (Data[cellIndex] >> bitIndex & _maxValue);
        }

        public void Set(int index, int value)
        {
            if (index < 0 || index > _size - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (value < 0 || value > _maxValue)
            {
                throw new ArgumentException("Value cannot be outside of accepted range.");
            }
            var cellIndex = CellIndex(index);
            var bitIndex = BitIndex(index, cellIndex);
            Data[cellIndex] = Data[cellIndex] & ~(_maxValue << bitIndex) |
                               (value & _maxValue) << bitIndex;
        }

        public int[] ToIntArray()
        {
            var result = new int[_size];
            var index = 0;
            foreach (var rawCell in Data)
            {
                var cell = rawCell;
                for (var bitIndex = 0; bitIndex < _valuesPerLong; bitIndex++)
                {
                    result[index++] = (int) (cell & _maxValue);
                    cell >>= BitsPerEntry;
                    if (index >= _size)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private int CellIndex(int index)
        {
            long i = index;
            return (int) (i * _divideMultiply + _divideAdd >> 32 >> _divideShift);
        }

        private int BitIndex(int index, int cellIndex)
        {
            return (index - cellIndex * _valuesPerLong) * BitsPerEntry;
        }
    }
}