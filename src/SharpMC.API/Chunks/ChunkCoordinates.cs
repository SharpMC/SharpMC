using System;
using SharpMC.API.Players;
using SharpMC.API.Utils;
using SharpMC.API.Worlds;

namespace SharpMC.API.Chunks
{
    public readonly record struct ChunkCoordinates(int X, int Z)
    {
        public ChunkCoordinates(int value) : this(value, value)
        {
        }

        public ChunkCoordinates(PlayerLocation loc)
            : this((int) Math.Floor(loc.X) >> 4, (int) Math.Floor(loc.Z) >> 4)
        {
        }

        #region Math

        /// <summary>
        /// Calculates the distance between two ChunkCoordinates objects
        /// </summary>
        public double DistanceTo(ChunkCoordinates other)
            => Math.Sqrt(Square(other.X - X) + Square(other.Z - Z));

        /// <summary>
        /// Calculates the square of a num
        /// </summary>
        private static int Square(int num) => num * num;

        /// <summary>
        /// Finds the distance of this ChunkCoordinates from ChunkCoordinates.Zero
        /// </summary>
        public double Distance => DistanceTo(Zero);

        public static ChunkCoordinates Min(ChunkCoordinates a, ChunkCoordinates b) =>
            new(Math.Min(a.X, b.X), Math.Min(a.Z, b.Z));

        public static ChunkCoordinates Max(ChunkCoordinates a, ChunkCoordinates b) =>
            new(Math.Max(a.X, b.X), Math.Max(a.Z, b.Z));

        #endregion

        #region Operators

        public static ChunkCoordinates operator +(ChunkCoordinates a, ChunkCoordinates b)
            => new(a.X + b.X, a.Z + b.Z);

        public static ChunkCoordinates operator -(ChunkCoordinates a, ChunkCoordinates b)
            => new(a.X - b.X, a.Z - b.Z);

        public static ChunkCoordinates operator -(ChunkCoordinates a)
            => new(-a.X, -a.Z);

        public static ChunkCoordinates operator *(ChunkCoordinates a, ChunkCoordinates b)
            => new(a.X * b.X, a.Z * b.Z);

        public static ChunkCoordinates operator /(ChunkCoordinates a, ChunkCoordinates b)
            => new(a.X / b.X, a.Z / b.Z);

        public static ChunkCoordinates operator %(ChunkCoordinates a, ChunkCoordinates b)
            => new(a.X % b.X, a.Z % b.Z);

        public static ChunkCoordinates operator +(ChunkCoordinates a, int b)
            => new(a.X + b, a.Z + b);

        public static ChunkCoordinates operator -(ChunkCoordinates a, int b)
            => new(a.X - b, a.Z - b);

        public static ChunkCoordinates operator *(ChunkCoordinates a, int b)
            => new(a.X * b, a.Z * b);

        public static ChunkCoordinates operator /(ChunkCoordinates a, int b)
            => new(a.X / b, a.Z / b);

        public static ChunkCoordinates operator %(ChunkCoordinates a, int b)
            => new(a.X % b, a.Z % b);

        public static ChunkCoordinates operator +(int a, ChunkCoordinates b)
            => new(a + b.X, a + b.Z);

        public static ChunkCoordinates operator -(int a, ChunkCoordinates b)
            => new(a - b.X, a - b.Z);

        public static ChunkCoordinates operator *(int a, ChunkCoordinates b)
            => new(a * b.X, a * b.Z);

        public static ChunkCoordinates operator /(int a, ChunkCoordinates b)
            => new(a / b.X, a / b.Z);

        public static ChunkCoordinates operator %(int a, ChunkCoordinates b)
            => new(a % b.X, a % b.Z);

        #endregion

        #region Constants

        public static readonly ChunkCoordinates Zero = new(0);
        public static readonly ChunkCoordinates One = new(1);

        public static readonly ChunkCoordinates Forward = new(0, 1);
        public static readonly ChunkCoordinates Backward = new(0, -1);
        public static readonly ChunkCoordinates Left = new(-1, 0);
        public static readonly ChunkCoordinates Right = new(1, 0);

        #endregion
    }
}