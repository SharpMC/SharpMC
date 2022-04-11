using System;

namespace SharpMC.Util
{
    public class VariableValueArray
    {
        public long[] Backing { get; }
        public int Capacity { get; }
        public int BitsPerValue { get; }

        private readonly long _valueMask;

        public VariableValueArray(int bitsPerValue, int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException($"capacity {capacity} must not be negative");
            }
            if (bitsPerValue < 1)
            {
                throw new ArgumentException($"bitsPerValue {bitsPerValue} must not be smaller then 1");
            }
            if (bitsPerValue > 64)
            {
                throw new ArgumentException($"bitsPerValue {bitsPerValue} must not be greater then 64");
            }
            Backing = new long[(int) Math.Ceiling((bitsPerValue * capacity) / 64.0)];
            BitsPerValue = bitsPerValue;
            _valueMask = (1L << bitsPerValue) - 1L;
            Capacity = capacity;
        }

        public int this[int index]
        {
            get
            {
                CheckIndex(index);
                index *= BitsPerValue;
                var i0 = index >> 6;
                var i1 = index & 0x3f;
                var value = Backing[i0] >> i1;
                var i2 = i1 + BitsPerValue;
                // The value is divided over two long values
                if (i2 > 64)
                {
                    value |= Backing[++i0] << 64 - i1;
                }
                return (int) (value & _valueMask);
            }
            set
            {
                CheckIndex(index);
                if (value < 0)
                {
                    throw new ArgumentException($"value {value} must not be negative");
                }
                if (value > _valueMask)
                {
                    throw new ArgumentException($"value {value} must not be greater then {_valueMask}");
                }
                index *= BitsPerValue;
                var i0 = index >> 6;
                var i1 = index & 0x3f;
                Backing[i0] = Backing[i0] & ~(_valueMask << i1) | (value & _valueMask) << i1;
                var i2 = i1 + BitsPerValue;
                // The value is divided over two long values
                if (i2 > 64)
                {
                    i0++;
                    Backing[i0] = Backing[i0] & ~((1L << i2 - 64) - 1L) | (uint) (value >> 64 - i1);
                }
            }
        }

        private void CheckIndex(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException($"index {index} must not be negative");
            }

            if (index >= Capacity)
            {
                throw new IndexOutOfRangeException($"index {index} must not be greater then the capacity {Capacity}");
            }
        }
    }
}