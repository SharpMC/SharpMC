using System.Numerics;

namespace SharpMC.Core.Utils
{
	public class Ray2
	{
		/**
     * Epsilon
     */
		public static double EPSILON = 0.5;

		/**
     * Offset
     */
		public static double OFFSET = 0.0001;

		/**
     * Origin of the Ray
     */
		public Vector3 x;

		/**
     * Direction of the Ray
     */
		public Vector3 d;

		/**
     * Normal vector of intersection
     */
		public Vector3 n = new Vector3();

		/**
     * tNear variable
     */
		public double tNear;

		/**
     * t variable / parameter of ray
     */
		public double t;

		/**
     * Texture coordinate
     */
		public double u;

		/**
     * Texture coordinate
     */
		public double v;
	}
}
