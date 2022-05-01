using System.Numerics;
using SharpMC.API.Blocks;

namespace SharpMC.API.Utils
{
    public interface ILocation
    {
        Vector3 ToVector3();
        Vector3 GetHeadDirection();
        Vector3 GetDirection();

        float X { get; }
        float Y { get; }
        float Z { get; }
        float Yaw { get; }
        float HeadYaw { get; }
        float Pitch { get; }
        bool OnGround { get; }

        BlockCoordinates GetCoordinates3D();

        double DistanceTo(ILocation other);
    }
}