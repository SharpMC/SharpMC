using LibNoise;
using LibNoise.Primitive;
using SharpMC.Noise.API;

namespace SharpMC.Noise.Lib
{
    public class GcRandom : IGcRandom
    {
        // Density
        private readonly int _amplitude1 = 100;
        private readonly int _amplitude2 = 2;

        private readonly int _amplitude3 = 20;

        // Position
        private readonly int _caveBandBuffer;
        private readonly int _cutOff = 62;
        private readonly double _f1Xz;

        private readonly double _f1Y;

        // Second pass - small noise
        private readonly double _f2Xz = 0.25;

        private readonly double _f2Y = 0.05;

        // Third pass - vertical noise
        private readonly double _f3Xz = 0.025;
        private readonly double _f3Y = 0.005;
        private readonly int _maxCaveHeight = 50;
        private readonly int _minCaveHeight = 6;
        private readonly SimplexPerlin _noiseGen1;
        private readonly SimplexPerlin _noiseGen2;
        private readonly SimplexPerlin _noiseGen3;
        private readonly double _subtractForLessThanCutoff;
        private readonly int _sxz = 200;
        private readonly int _sy = 100;

        private (int X, int Z) Pos { get; }

        public GcRandom(int seed, (int X, int Z) pos)
        {
            var chunkX = pos.X;
            var chunkZ = pos.Z;

            Pos = pos;
            _subtractForLessThanCutoff = _amplitude1 - _cutOff;
            _f1Xz = 1.0 / _sxz;
            _f1Y = 1.0 / _sy;

            if (_maxCaveHeight - _minCaveHeight > 128)
            {
                _caveBandBuffer = 32;
            }
            else
            {
                _caveBandBuffer = 16;
            }

            _noiseGen1 = new SimplexPerlin(seed, NoiseQuality.Fast);
            _noiseGen2 = new SimplexPerlin((int) _noiseGen1.GetValue(chunkX, chunkZ), NoiseQuality.Fast);
            _noiseGen3 = new SimplexPerlin((int) _noiseGen1.GetValue(chunkX, chunkZ), NoiseQuality.Fast);
        }

        public bool IsInCave(int x, int y, int z)
        {
            var chunkX = Pos.X;
            var chunkZ = Pos.Z;

            float xx = (chunkX << 4) | (x & 0xF);
            float yy = y;
            float zz = (chunkZ << 4) | (z & 0xF);

            double n1 = _noiseGen1.GetValue((float) (xx * _f1Xz), (float) (yy * _f1Y), (float) (zz * _f1Xz)) *
                        _amplitude1;
            double n2 = _noiseGen2.GetValue((float) (xx * _f2Xz), (float) (yy * _f2Y), (float) (zz * _f2Xz)) *
                        _amplitude2;
            double n3 = _noiseGen3.GetValue((float) (xx * _f3Xz), (float) (yy * _f3Y), (float) (zz * _f3Xz)) *
                        _amplitude3;
            var lc = LinearCutoffCoefficient(y);

            var isInCave = n1 + n2 - n3 - lc > 62;
            return isInCave;
        }

        private double LinearCutoffCoefficient(int y)
        {
            // Out of bounds
            if (y < _minCaveHeight || y > _maxCaveHeight)
            {
                return _subtractForLessThanCutoff;
                // Bottom layer distortion
            }
            if (y >= _minCaveHeight && y <= _minCaveHeight + _caveBandBuffer)
            {
                double yy = y - _minCaveHeight;
                return -_subtractForLessThanCutoff / _caveBandBuffer * yy + _subtractForLessThanCutoff;
                // Top layer distortion
            }
            if (y <= _maxCaveHeight && y >= _maxCaveHeight - _caveBandBuffer)
            {
                double yy = y - _maxCaveHeight + _caveBandBuffer;
                return _subtractForLessThanCutoff / _caveBandBuffer * yy;
                // In bounds, no distortion
            }
            return 0;
        }
    }
}