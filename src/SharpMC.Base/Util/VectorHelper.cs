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
			var distance = Math.Sqrt((vector.X * vector.X) + (vector.Z * vector.Z));
			return ToDegrees(Math.Atan2(vector.Y, distance));
		}

		public static double ToRadians(this float angle)
		{
			return (Math.PI / 180.0f) * angle;
		}

		public static double ToDegrees(this double angle)
		{
			return angle * (180.0f / Math.PI);
		}
	}
}
