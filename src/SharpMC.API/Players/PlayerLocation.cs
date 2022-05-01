using System;
using System.Numerics;
using SharpMC.API.Blocks;
using SharpMC.API.Utils;

namespace SharpMC.API.Players
{
    public readonly record struct PlayerLocation(float X, float Y, float Z,
        float Yaw, float Pitch, float HeadYaw, bool OnGround) : ILocation
    {
        public PlayerLocation(float x, float y, float z,
            float headYaw = 0f, float yaw = 0f, float pitch = 0f)
            : this(x, y, z, yaw, pitch, headYaw, default)
        {
        }

        public PlayerLocation(Vector3 vector, float headYaw = 0f,
            float yaw = 0f, float pitch = 0f)
            : this(vector.X, vector.Y, vector.Z, headYaw, yaw, pitch)
        {
        }

        public PlayerLocation(double x, double y, double z,
            float headYaw = 0f, float yaw = 0f, float pitch = 0f)
            : this((float) x, (float) y, (float) z, headYaw, yaw, pitch)
        {
        }

        public PlayerLocation(ILocation loc) : this(loc.X, loc.Y, loc.Z,
            loc.Yaw, loc.Pitch, loc.HeadYaw, loc.OnGround)
        {
        }

        public Vector3 ToVector3() => new(X, Y, Z);

        public Vector3 GetHeadDirection()
        {
            var vector = new Vector3();
            var pitch = Pitch.ToRadians();
            var yaw = HeadYaw.ToRadians();
            vector.X = (float) (-Math.Sin(yaw) * Math.Cos(pitch));
            vector.Y = (float) -Math.Sin(pitch);
            vector.Z = (float) (Math.Cos(yaw) * Math.Cos(pitch));
            return vector;
        }

        public Vector3 GetDirection()
        {
            var vector = new Vector3();
            var pitch = Pitch.ToRadians();
            var yaw = Yaw.ToRadians();
            vector.X = (float) (-Math.Sin(yaw) * Math.Cos(pitch));
            vector.Y = (float) -Math.Sin(pitch);
            vector.Z = (float) (Math.Cos(yaw) * Math.Cos(pitch));
            return vector;
        }

        public BlockCoordinates GetCoordinates3D() 
            => new((int) X, (int) Y, (int) Z);

        public double DistanceTo(ILocation other)
        {
            return Math.Sqrt(Square(other.X - X) +
                             Square(other.Y - Y) +
                             Square(other.Z - Z));
        }

        private static double Square(double num) => num * num;
    }
}