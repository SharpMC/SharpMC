using System;
using System.Numerics;
using SharpMC.API.Chunks;
using SharpMC.API.Players;

namespace SharpMC.API.Blocks
{
    public readonly record struct BlockCoordinates(int X, int Y, int Z)
    {
        public BlockCoordinates(int value) : this(value, value, value)
        {
        }

        public BlockCoordinates(PlayerLocation loc) : this((int) Math.Floor(loc.X),
            (int) Math.Floor(loc.Y), (int) Math.Floor(loc.Z))
        {
        }

        /// <summary>
        /// Calculates the distance between two BlockCoordinates objects.
        /// </summary>
        public double DistanceTo(BlockCoordinates other)
            => Math.Sqrt(Square(other.X - X) +
                         Square(other.Y - Y) +
                         Square(other.Z - Z));

        /// <summary>
        /// Calculates the square of a num.
        /// </summary>
        private static int Square(int num) => num * num;

        /// <summary>
        /// Finds the distance of this Coordinate3D from BlockCoordinates.Zero
        /// </summary>
        public double Distance => DistanceTo(Zero);

        public static BlockCoordinates Min(BlockCoordinates a, BlockCoordinates b)
            => new(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y),
                Math.Min(a.Z, b.Z)
            );

        public static BlockCoordinates Max(BlockCoordinates a, BlockCoordinates b)
            => new(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y),
                Math.Max(a.Z, b.Z)
            );

        public static BlockCoordinates operator +(BlockCoordinates a, BlockCoordinates b)
            => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static BlockCoordinates operator -(BlockCoordinates a, BlockCoordinates b)
            => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static BlockCoordinates operator -(BlockCoordinates a)
            => new(-a.X, -a.Y, -a.Z);

        public static BlockCoordinates operator *(BlockCoordinates a, BlockCoordinates b)
            => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static BlockCoordinates operator /(BlockCoordinates a, BlockCoordinates b)
            => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static BlockCoordinates operator %(BlockCoordinates a, BlockCoordinates b)
            => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

        public static BlockCoordinates operator +(BlockCoordinates a, int b)
            => new(a.X + b, a.Y + b, a.Z + b);

        public static BlockCoordinates operator -(BlockCoordinates a, int b)
            => new(a.X - b, a.Y - b, a.Z - b);

        public static BlockCoordinates operator *(BlockCoordinates a, int b)
            => new(a.X * b, a.Y * b, a.Z * b);

        public static BlockCoordinates operator /(BlockCoordinates a, int b)
            => new(a.X / b, a.Y / b, a.Z / b);

        public static BlockCoordinates operator %(BlockCoordinates a, int b)
            => new(a.X % b, a.Y % b, a.Z % b);

        public static BlockCoordinates operator +(int a, BlockCoordinates b)
            => new(a + b.X, a + b.Y, a + b.Z);

        public static BlockCoordinates operator -(int a, BlockCoordinates b)
            => new(a - b.X, a - b.Y, a - b.Z);

        public static BlockCoordinates operator *(int a, BlockCoordinates b)
            => new(a * b.X, a * b.Y, a * b.Z);

        public static BlockCoordinates operator /(int a, BlockCoordinates b)
            => new(a / b.X, a / b.Y, a / b.Z);

        public static BlockCoordinates operator %(int a, BlockCoordinates b)
            => new(a % b.X, a % b.Y, a % b.Z);

        public static explicit operator BlockCoordinates(ChunkCoordinates a)
            => new(a.X, 0, a.Z);

        public static implicit operator BlockCoordinates(Vector3 a)
            => new((int) Math.Floor(a.X), (int) Math.Floor(a.Y), (int) Math.Floor(a.Z));

        public static explicit operator BlockCoordinates(PlayerLocation a)
            => new((int) Math.Floor(a.X), (int) Math.Floor(a.Y), (int) Math.Floor(a.Z));

        public static implicit operator Vector3(BlockCoordinates a)
            => new(a.X, a.Y, a.Z);

        public static readonly BlockCoordinates Zero = new(0);
        public static readonly BlockCoordinates One = new(1);

        public static readonly BlockCoordinates Up = new(0, 1, 0);
        public static readonly BlockCoordinates Down = new(0, -1, 0);
        public static readonly BlockCoordinates Left = new(-1, 0, 0);
        public static readonly BlockCoordinates Right = new(1, 0, 0);
        public static readonly BlockCoordinates Backwards = new(0, 0, -1);
        public static readonly BlockCoordinates Forwards = new(0, 0, 1);

        public static readonly BlockCoordinates East = new(1, 0, 0);
        public static readonly BlockCoordinates West = new(-1, 0, 0);
        public static readonly BlockCoordinates North = new(0, 0, -1);
        public static readonly BlockCoordinates South = new(0, 0, 1);
    }
}