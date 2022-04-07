using System.Numerics;

namespace SharpMC.Util
{
    internal class Vectors
    {
        public static readonly Vector3 Zero = new Vector3(0);
        public static readonly Vector3 One = new Vector3(1);

        public static readonly Vector3 Up = new Vector3(0, 1, 0);
        public static readonly Vector3 Down = new Vector3(0, -1, 0);
        public static readonly Vector3 Left = new Vector3(-1, 0, 0);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Backwards = new Vector3(0, 0, -1);
        public static readonly Vector3 Forwards = new Vector3(0, 0, 1);

        public static readonly Vector3 East = new Vector3(1, 0, 0);
        public static readonly Vector3 West = new Vector3(-1, 0, 0);
        public static readonly Vector3 North = new Vector3(0, 0, -1);
        public static readonly Vector3 South = new Vector3(0, 0, 1);

        public static Vector3 Create(double dX, double dY, double dZ)
        {
            var x = (float) dX;
            var y = (float) dY;
            var z = (float) dZ;
            return new Vector3(x, y, z);
        }
    }
}