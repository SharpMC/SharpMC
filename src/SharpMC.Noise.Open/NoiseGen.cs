using System;

namespace SharpMC.Noise.Open
{
    internal abstract class NoiseGen
    {
        protected abstract double Value2D(double x, double y);

        public abstract double Value3D(double x, double y, double z);

        protected int Floor(double value)
        {
            return value >= 0.0 ? (int) value : (int) value - 1;
        }

        private static double Interpolate(double pointA, double pointB, double t, InterpolateType type)
        {
            switch (type)
            {
                case InterpolateType.Cosine:
                    return CosineInterpolate(pointA, pointB, t);
                case InterpolateType.Linear:
                    return LinearInterpolate(pointA, pointB, t);
                default:
                    return LinearInterpolate(pointA, pointB, t);
            }
        }

        private static double CosineInterpolate(double pointA, double pointB, double t)
        {
            var f = t * Math.PI;
            var g = (1 - Math.Cos(f)) * 0.5;
            return pointA * (1 - g) + pointB * g;
        }

        private static double CubicInterpolate(double pointA, double pointB, double pointC, double pointD, double t)
        {
            var e = pointD - pointC - (pointA - pointB);
            var f = pointA - pointB - e;
            var g = pointC - pointA;
            var h = pointB;
            return e * Math.Pow(t, 3) + f * Math.Pow(t, 2) + g * t + h;
        }

        private static double LinearInterpolate(double pointA, double pointB, double t)
        {
            return pointA * (1 - t) + pointB * t;
        }

        private static double BiLinearInterpolate(double x, double y, double point00, double point01, double point10,
            double point11)
        {
            var point0 = LinearInterpolate(point00, point10, x);
            var point1 = LinearInterpolate(point01, point11, x);

            return LinearInterpolate(point0, point1, y);
        }

        private static double TriLinearInterpolate(double x, double y, double z,
            double point000, double point001, double point010,
            double point100, double point011, double point101,
            double point110, double point111)
        {
            var point0 = BiLinearInterpolate(x, y, point000, point001, point100, point101);
            var point1 = BiLinearInterpolate(x, y, point010, point011, point110, point111);

            return LinearInterpolate(point0, point1, z);
        }
    }
}