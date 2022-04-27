using System.Numerics;

namespace SharpMC.API.Utils
{
    public interface IPosition
    {
        Vector3 ToVector3();
        
        double Yaw { get; set; }
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
        float Pitch { get; set; }
        bool OnGround { get; }
    }
}