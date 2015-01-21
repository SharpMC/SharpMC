using System;

namespace SharpMCRewrite
{
    public class Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3 (double _X, double _Y, double _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }

        public string GetString()
        {
            return X + ", " + Y + ", " + Z;
        }
    }

    public class INTVector3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public INTVector3 (int _X, int _Y, int _Z)
        {
            X = _X;
            Y = _Y;
            Z = _Z;
        }
    }
}

