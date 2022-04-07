﻿using System;

namespace SharpMC.Util
{
    public class VoronoiNoiseGenerator
    {
        private const double Sqrt2 = 1.4142135623730950488;
        private const double Sqrt3 = 1.7320508075688772935;

	    private int _seed;

	    public int Seed
	    {
		    get
		    {
			    return _seed;
		    }
		    set
		    {
			    var val = value;
			    _seed = val;
			    Random = new Random(val);
		    }
	    }

	    private readonly DistanceMethod _distanceMethod;
	    private readonly bool _useDistance;

		private Random Random { get; set; }
        public VoronoiNoiseGenerator(int seed, DistanceMethod distanceMethod)
        {
            Seed = seed;
            _distanceMethod = distanceMethod;
	        _useDistance = distanceMethod != DistanceMethod.None;
        }

        private double GetDistance(double xDist, double zDist)
        {
            switch (_distanceMethod)
            {
                case DistanceMethod.Sqrt:
                    return Math.Sqrt(xDist * xDist + zDist * zDist) / Sqrt2;
                case DistanceMethod.Simple:
                    return xDist + zDist;
                default:
                    return Double.NaN;
            }
        }

        private double GetDistance(double xDist, double yDist, double zDist)
        {
            switch (_distanceMethod)
            {
				case DistanceMethod.Sqrt:
					return Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist) / Sqrt3; //Approximation (for speed) of elucidean (regular) distance
				case DistanceMethod.Simple:
					return xDist + yDist + zDist;
                default:
                    return Double.NaN;
            }
        }

        public double Noise(double x, double z, double frequency)
        {
            x *= frequency;
            z *= frequency;

            var xInt = x > .0 ? (int)x : (int)x - 1;
            var zInt = z > .0 ? (int)z : (int)z - 1;

            var minDist = 32000000.0;

            double xCandidate = 0;
            double zCandidate = 0;

            for (var zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (var xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                {
                    var xPos = xCur + ValueNoise2D(xCur, zCur, Seed);
                    var zPos = zCur + ValueNoise2D(xCur, zCur, Random.NextLong());
                    var xDist = xPos - x;
                    var zDist = zPos - z;
                    var dist = xDist * xDist + zDist * zDist;

                    if (dist < minDist)
                    {
                        minDist = dist;
                        xCandidate = xPos;
                        zCandidate = zPos;
                    }
                }
            }

            if (_useDistance)
            {
                var xDist = xCandidate - x;
                var zDist = zCandidate - z;
                return GetDistance(xDist, zDist);
            }

            return ValueNoise2D(
                (int)Math.Floor(xCandidate),
                (int)Math.Floor(zCandidate), Seed);
        }

        public double Noise(double x, double y, double z, double frequency)
        {
            // Inside each unit cube, there is a _seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a _seed point
            // that is closest to the specified position.
            x *= frequency;
            y *= frequency;
            z *= frequency;

            var xInt = x > .0 ? (int)x : (int)x - 1;
            var yInt = y > .0 ? (int)y : (int)y - 1;
            var zInt = z > .0 ? (int)z : (int)z - 1;

            var minDist = 32000000.0;

            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;

            for (var zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (var yCur = yInt - 2; yCur <= yInt + 2; yCur++)
                {
                    for (var xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                    {
                        // Calculate the position and distance to the _seed point inside of
                        // this unit cube.

                        var xPos = xCur + ValueNoise3D(xCur, yCur, zCur, Seed);
                        var yPos = yCur + ValueNoise3D(xCur, yCur, zCur, Random.NextLong());
	                    var zPos = zCur + ValueNoise3D(xCur, yCur, zCur, Random.NextLong());
                        var xDist = xPos - x;
                        var yDist = yPos - y;
                        var zDist = zPos - z;
                        var dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This _seed point is closer to any others found so far, so record
                            // this _seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

			if (_useDistance)
			{
                var xDist = xCandidate - x;
                var yDist = yCandidate - y;
                var zDist = zCandidate - z;

                return GetDistance(xDist, yDist, zDist);
            }

            return ValueNoise3D(
                (int)Math.Floor(xCandidate),
                (int)Math.Floor(yCandidate),
                (int)Math.Floor(zCandidate), Seed);
        }

        /**
         * To avoid having to store the feature points, we use a hash function 
         * of the coordinates and the _seed instead. Those big scary numbers are
         * arbitrary primes.
         */
        public static double ValueNoise2D(int x, int z, long seed)
        {
            var n = (1619 * x + 6971 * z + 1013 * seed) & 0x7fffffff;
            n = (n >> 13) ^ n;
            return 1.0 - ((n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff) / 1073741824.0;
        }

        public static double ValueNoise3D(int x, int y, int z, long seed)
        {
            var n = (1619 * x + 31337 * y + 6971 * z + 1013 * seed) & 0x7fffffff;
            n = (n >> 13) ^ n;
            return 1.0 - ((n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff) / 1073741824.0;
        }

	    public enum DistanceMethod
	    {
		    Sqrt,
			Simple,
			None
	    }
    }
}
