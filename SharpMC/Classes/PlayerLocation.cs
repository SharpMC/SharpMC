using System;

namespace SharpMC.Classes
{
	public class PlayerLocation : Vector3
	{
		public float Yaw { get; set; }
		public float Pitch { get; set; }
		public bool OnGround { get; set; }

		public PlayerLocation(double _X, double _Y, double _Z) : base(_X, _Y, _Z)
		{
		}

		public double DistanceTo(PlayerLocation other)
		{
			return Math.Sqrt(Square(other.X - X) +
							 Square(other.Y - Y) +
							 Square(other.Z - Z));
		}

		private double Square(double num)
		{
			return num * num;
		}
	}
}
