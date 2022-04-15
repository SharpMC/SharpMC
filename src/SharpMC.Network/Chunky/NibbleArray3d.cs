using SharpMC.Network.Util;

namespace SharpMC.Chunky
{
    public class NibbleArray3d
    {
        private readonly byte[] _data;

        public NibbleArray3d(byte[] data)
        {
            _data = data;
        }

        public NibbleArray3d(int size) 
            : this(new byte[size >> 1])
        {
        }

        public NibbleArray3d(IMinecraftReader input, int size) 
            : this(input.Read(size))
        {
        }

        public void Write(IMinecraftWriter output)
        {
            output.Write(_data);
        }

        public int Get(int x, int y, int z)
        {
            var key = y << 8 | z << 4 | x;
            var index = key >> 1;
            var part = key & 1;
            return part == 0 ? _data[index] & 15 : _data[index] >> 4 & 15;
        }

        public void Set(int x, int y, int z, int val)
        {
            var key = y << 8 | z << 4 | x;
            var index = key >> 1;
            var part = key & 1;
            if (part == 0)
            {
                _data[index] = (byte) (_data[index] & 240 | val & 15);
            }
            else
            {
                _data[index] = (byte) (_data[index] & 15 | (val & 15) << 4);
            }
        }

        public void Fill(int val)
        {
            for (var index = 0; index < _data.Length << 1; index++)
            {
                var ind = index >> 1;
                var part = index & 1;
                if (part == 0)
                {
                    _data[ind] = (byte) (_data[ind] & 240 | val & 15);
                }
                else
                {
                    _data[ind] = (byte) (_data[ind] & 15 | (val & 15) << 4);
                }
            }
        }
    }
}