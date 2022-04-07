using System;
using System.Numerics;

namespace SharpMC.Util
{
    public static class VectorHelpers
    {
        public static double GetYaw(this Vector3 vector)
        {
            return ToDegrees(Math.Atan2(vector.X, vector.Z));
        }

        public static double GetPitch(this Vector3 vector)
        {
            var distance = Math.Sqrt(vector.X * vector.X + vector.Z * vector.Z);
            return ToDegrees(Math.Atan2(vector.Y, distance));
        }

        public static double ToRadians(this float angle)
        {
            return Math.PI / 180.0f * angle;
        }

        public static double ToDegrees(this double angle)
        {
            return angle * (180.0f / Math.PI);
        }

        public static PlayerLocation ToPlayerLocation(this Vector3 v)
        {
            return new PlayerLocation(v.X, v.Y, v.Z);
        }

        public static Vector3 Copy(this Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Tuple<int, int> ToTuple(this Vector2 v)
        {
            var tX = (int) v.X;
            var tY = (int) v.Y;
            return new Tuple<int, int>(tX, tY);
        }

        /// <summary>
        ///     Calculates the distance between two Vector3 objects.
        /// </summary>
        public static double DistanceTo(this Vector3 v, Vector3 other)
        {
            return Math.Sqrt(Square(other.X - v.X) +
                             Square(other.Y - v.Y) +
                             Square(other.Z - v.Z));
        }

        /// <summary>
        ///     Calculates the square of a num.
        /// </summary>
        private static double Square(double num)
        {
            return num * num;
        }

        /// <summary>
        ///     Finds the distance of this vector from Vector3.Zero
        /// </summary>
        public static double Distance(this Vector3 v)
        {
            return v.DistanceTo(Vectors.Zero);
        }

        public static Vector3 Normalize(this Vector3 v)
        {
            return Vectors.Create(v.X / v.Distance(), v.Y / v.Distance(),
                v.Z / v.Distance());
        }

        /// <summary>
        ///     Truncates the decimal component of each part of this Vector3.
        /// </summary>
        public static Vector3 Floor(this Vector3 v)
        {
            return Vectors.Create(Math.Floor(v.X), Math.Floor(v.Y),
                Math.Floor(v.Z));
        }

        public static Vector3 Subtract(this Vector3 v, double n)
        {
            throw new NotImplementedException();
        }

        public static Vector3 Add(this Vector3 v, double n)
        {
            throw new NotImplementedException();
        }

        public static Vector3 Mult(this Vector3 v, double n)
        {
            throw new NotImplementedException();
        }
    }
}