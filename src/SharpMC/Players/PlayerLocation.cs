using SharpMC.API.Utils;
using System;
using System.Numerics;
using SharpMC.Util;

namespace SharpMC.Players
{
	// Credits to https://github.com/NiclasOlofsson/MiNET
	public class PlayerLocation : ICloneable, IPosition
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float Yaw { get; set; }
		public float Pitch { get; set; }
		public float HeadYaw { get; set; }

		public PlayerLocation()
		{
		}

		public PlayerLocation(float x, float y, float z, float headYaw = 0f, float yaw = 0f, float pitch = 0f)
		{
			X = x;
			Y = y;
			Z = z;
			HeadYaw = headYaw;
			Yaw = yaw;
			Pitch = pitch;
		}

		public PlayerLocation(double x, double y, double z, float headYaw = 0f, float yaw = 0f, float pitch = 0f)
			: this((float) x, (float) y, (float) z, headYaw, yaw, pitch)
		{
		}

		public PlayerLocation(Vector3 vector, float headYaw = 0f, float yaw = 0f, float pitch = 0f)
			: this(vector.X, vector.Y, vector.Z, headYaw, yaw, pitch)
		{
		}

		public BlockCoordinates GetCoordinates3D()
		{
			return new BlockCoordinates((int) X, (int) Y, (int) Z);
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

		public Vector3 GetDirection()
		{
			var vector = new Vector3();
			double pitch = Pitch.ToRadians();
			double yaw = Yaw.ToRadians();
			vector.X = (float) (-Math.Sin(yaw) * Math.Cos(pitch));
			vector.Y = (float) -Math.Sin(pitch);
			vector.Z = (float) (Math.Cos(yaw) * Math.Cos(pitch));
			return vector;
		}

		public Vector3 GetHeadDirection()
		{
			var vector = new Vector3();
			double pitch = Pitch.ToRadians();
			double yaw = HeadYaw.ToRadians();
			vector.X = (float) (-Math.Sin(yaw) * Math.Cos(pitch));
			vector.Y = (float) -Math.Sin(pitch);
			vector.Z = (float) (Math.Cos(yaw) * Math.Cos(pitch));
			return vector;
		}

		Vector3 IPosition.ToVector3()
		{
			return new Vector3(X, Y, Z);
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		IPosition IPosition.Clone()
		{
			return new PlayerLocation(X, Y, Z);
		}

		public override string ToString()
		{
			return $"X={X}, Y={Y}, Z={Z}, HeadYaw={HeadYaw}, Yaw={Yaw}, Pich={Pitch}, ";
		}
	}
}