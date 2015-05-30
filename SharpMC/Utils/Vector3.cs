// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using System;

namespace SharpMC.Utils
{
	public struct Vector3 : IEquatable<Vector3>
	{
		public double X;
		public double Y;
		public double Z;

		public Vector3(double value)
		{
			X = Y = Z = value;
		}

		public Vector3(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(Vector3 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		///     Finds the distance of this vector from Vector3.Zero
		/// </summary>
		public double Distance
		{
			get { return DistanceTo(Zero); }
		}

		public bool Equals(Vector3 other)
		{
			return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
		}

		/// <summary>
		///     Truncates the decimal component of each part of this Vector3.
		/// </summary>
		public Vector3 Floor()
		{
			return new Vector3(Math.Floor(X), Math.Floor(Y), Math.Floor(Z));
		}

		public Vector3 Normalize()
		{
			return new Vector3(X/Distance, Y/Distance, Z/Distance);
		}

		/// <summary>
		///     Calculates the distance between two Vector3 objects.
		/// </summary>
		public double DistanceTo(Vector3 other)
		{
			return Math.Sqrt(Square(other.X - X) +
			                 Square(other.Y - Y) +
			                 Square(other.Z - Z));
		}

		/// <summary>
		///     Calculates the square of a num.
		/// </summary>
		private double Square(double num)
		{
			return num*num;
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				Math.Min(value1.X, value2.X),
				Math.Min(value1.Y, value2.Y),
				Math.Min(value1.Z, value2.Z)
				);
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				Math.Max(value1.X, value2.X),
				Math.Max(value1.Y, value2.Y),
				Math.Max(value1.Z, value2.Z)
				);
		}

		public static bool operator !=(Vector3 a, Vector3 b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Vector3 a, Vector3 b)
		{
			return a.Equals(b);
		}

		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X + b.X,
				a.Y + b.Y,
				a.Z + b.Z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X - b.X,
				a.Y - b.Y,
				a.Z - b.Z);
		}

		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(
				-a.X,
				-a.Y,
				-a.Z);
		}

		public static Vector3 operator *(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X*b.X,
				a.Y*b.Y,
				a.Z*b.Z);
		}

		public static Vector3 operator /(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.X/b.X,
				a.Y/b.Y,
				a.Z/b.Z);
		}

		public static Vector3 operator %(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X%b.X, a.Y%b.Y, a.Z%b.Z);
		}

		public static Vector3 operator +(Vector3 a, double b)
		{
			return new Vector3(
				a.X + b,
				a.Y + b,
				a.Z + b);
		}

		public static Vector3 operator -(Vector3 a, double b)
		{
			return new Vector3(
				a.X - b,
				a.Y - b,
				a.Z - b);
		}

		public static Vector3 operator *(Vector3 a, double b)
		{
			return new Vector3(
				a.X*b,
				a.Y*b,
				a.Z*b);
		}

		public static Vector3 operator /(Vector3 a, double b)
		{
			return new Vector3(
				a.X/b,
				a.Y/b,
				a.Z/b);
		}

		public static Vector3 operator %(Vector3 a, double b)
		{
			return new Vector3(a.X%b, a.Y%b, a.Y%b);
		}

		public static Vector3 operator +(double a, Vector3 b)
		{
			return new Vector3(
				a + b.X,
				a + b.Y,
				a + b.Z);
		}

		public static Vector3 operator -(double a, Vector3 b)
		{
			return new Vector3(
				a - b.X,
				a - b.Y,
				a - b.Z);
		}

		public static Vector3 operator *(double a, Vector3 b)
		{
			return new Vector3(
				a*b.X,
				a*b.Y,
				a*b.Z);
		}

		public static Vector3 operator /(double a, Vector3 b)
		{
			return new Vector3(
				a/b.X,
				a/b.Y,
				a/b.Z);
		}

		public static Vector3 operator %(double a, Vector3 b)
		{
			return new Vector3(a%b.X, a%b.Y, a%b.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Vector3)) return false;
			return Equals((Vector3) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = X.GetHashCode();
				result = (result*397) ^ Y.GetHashCode();
				result = (result*397) ^ Z.GetHashCode();
				return result;
			}
		}

		public PlayerLocation ToPlayerLocation()
		{
			return new PlayerLocation(X, Y, Z);
		}

		#region Constants

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

		#endregion
	}
}