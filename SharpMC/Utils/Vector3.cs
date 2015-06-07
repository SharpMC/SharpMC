#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC.Utils
{
	using System;

	public struct Vector3 : IEquatable<Vector3>
	{
		public double X;

		public double Y;

		public double Z;

		public Vector3(double value)
		{
			this.X = this.Y = this.Z = value;
		}

		public Vector3(double x, double y, double z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Vector3(Vector3 v)
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
		}

		/// <summary>
		///     Finds the distance of this vector from Vector3.Zero
		/// </summary>
		public double Distance
		{
			get
			{
				return this.DistanceTo(Zero);
			}
		}

		public bool Equals(Vector3 other)
		{
			return other.X.Equals(this.X) && other.Y.Equals(this.Y) && other.Z.Equals(this.Z);
		}

		/// <summary>
		/// Truncates the decimal component of each part of this Vector3.
		/// </summary>
		/// <returns>
		/// The <see cref="Vector3"/>.
		/// </returns>
		public Vector3 Floor()
		{
			return new Vector3(Math.Floor(this.X), Math.Floor(this.Y), Math.Floor(this.Z));
		}

		public Vector3 Normalize()
		{
			return new Vector3(this.X / this.Distance, this.Y / this.Distance, this.Z / this.Distance);
		}

		/// <summary>
		/// Calculates the distance between two Vector3 objects.
		/// </summary>
		/// <param name="other">
		/// The other.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		public double DistanceTo(Vector3 other)
		{
			return Math.Sqrt(this.Square(other.X - this.X) + this.Square(other.Y - this.Y) + this.Square(other.Z - this.Z));
		}

		/// <summary>
		/// Calculates the square of a num.
		/// </summary>
		/// <param name="num">
		/// The num.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double Square(double num)
		{
			return num * num;
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3(Math.Min(value1.X, value2.X), Math.Min(value1.Y, value2.Y), Math.Min(value1.Z, value2.Z));
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y), Math.Max(value1.Z, value2.Z));
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
			return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(-a.X, -a.Y, -a.Z);
		}

		public static Vector3 operator *(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}

		public static Vector3 operator /(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}

		public static Vector3 operator %(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
		}

		public static Vector3 operator +(Vector3 a, double b)
		{
			return new Vector3(a.X + b, a.Y + b, a.Z + b);
		}

		public static Vector3 operator -(Vector3 a, double b)
		{
			return new Vector3(a.X - b, a.Y - b, a.Z - b);
		}

		public static Vector3 operator *(Vector3 a, double b)
		{
			return new Vector3(a.X * b, a.Y * b, a.Z * b);
		}

		public static Vector3 operator /(Vector3 a, double b)
		{
			return new Vector3(a.X / b, a.Y / b, a.Z / b);
		}

		public static Vector3 operator %(Vector3 a, double b)
		{
			return new Vector3(a.X % b, a.Y % b, a.Y % b);
		}

		public static Vector3 operator +(double a, Vector3 b)
		{
			return new Vector3(a + b.X, a + b.Y, a + b.Z);
		}

		public static Vector3 operator -(double a, Vector3 b)
		{
			return new Vector3(a - b.X, a - b.Y, a - b.Z);
		}

		public static Vector3 operator *(double a, Vector3 b)
		{
			return new Vector3(a * b.X, a * b.Y, a * b.Z);
		}

		public static Vector3 operator /(double a, Vector3 b)
		{
			return new Vector3(a / b.X, a / b.Y, a / b.Z);
		}

		public static Vector3 operator %(double a, Vector3 b)
		{
			return new Vector3(a % b.X, a % b.Y, a % b.Y);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (obj.GetType() != typeof(Vector3))
			{
				return false;
			}

			return this.Equals((Vector3)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = this.X.GetHashCode();
				result = (result * 397) ^ this.Y.GetHashCode();
				result = (result * 397) ^ this.Z.GetHashCode();
				return result;
			}
		}

		public PlayerLocation ToPlayerLocation()
		{
			return new PlayerLocation(this.X, this.Y, this.Z);
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