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

	public struct Ray : IEquatable<Ray>
	{
		#region Public Constructors

		public Ray(Vector3 position, Vector3 direction)
		{
			this.Position = position;
			this.Direction = direction;
		}

		#endregion

		#region Public Fields

		public readonly Vector3 Direction;

		public readonly Vector3 Position;

		#endregion

		#region Public Methods

		public override bool Equals(object obj)
		{
			return (obj is Ray) && this.Equals((Ray)obj);
		}

		public bool Equals(Ray other)
		{
			return this.Position.Equals(other.Position) && this.Direction.Equals(other.Direction);
		}

		public override int GetHashCode()
		{
			return this.Position.GetHashCode() ^ this.Direction.GetHashCode();
		}

		public double? Intersects(BoundingBox box)
		{
			// first test if start in box
			if (this.Position.X >= box.Min.X && this.Position.X <= box.Max.X && this.Position.Y >= box.Min.Y
			    && this.Position.Y <= box.Max.Y && this.Position.Z >= box.Min.Z && this.Position.Z <= box.Max.Z)
			{
				return 0.0f; // here we concidere cube is full and origine is in cube so intersect at origine
			}

			// Second we check each face
			var maxT = new Vector3(-1.0f);

			// Vector3 minT = new Vector3(-1.0f);
			// calcul intersection with each faces
			if (this.Position.X < box.Min.X && this.Direction.X != 0.0f)
			{
				maxT.X = (box.Min.X - this.Position.X) / this.Direction.X;
			}
			else if (this.Position.X > box.Max.X && this.Direction.X != 0.0f)
			{
				maxT.X = (box.Max.X - this.Position.X) / this.Direction.X;
			}

			if (this.Position.Y < box.Min.Y && this.Direction.Y != 0.0f)
			{
				maxT.Y = (box.Min.Y - this.Position.Y) / this.Direction.Y;
			}
			else if (this.Position.Y > box.Max.Y && this.Direction.Y != 0.0f)
			{
				maxT.Y = (box.Max.Y - this.Position.Y) / this.Direction.Y;
			}

			if (this.Position.Z < box.Min.Z && this.Direction.Z != 0.0f)
			{
				maxT.Z = (box.Min.Z - this.Position.Z) / this.Direction.Z;
			}
			else if (this.Position.Z > box.Max.Z && this.Direction.Z != 0.0f)
			{
				maxT.Z = (box.Max.Z - this.Position.Z) / this.Direction.Z;
			}

			// get the maximum maxT
			if (maxT.X > maxT.Y && maxT.X > maxT.Z)
			{
				if (maxT.X < 0.0f)
				{
					return null; // ray go on opposite of face
				}

				// coordonate of hit point of face of cube
				var coord = this.Position.Z + maxT.X * this.Direction.Z;

				// if hit point coord ( intersect face with ray) is out of other plane coord it miss
				if (coord < box.Min.Z || coord > box.Max.Z)
				{
					return null;
				}

				coord = this.Position.Y + maxT.X * this.Direction.Y;
				if (coord < box.Min.Y || coord > box.Max.Y)
				{
					return null;
				}

				return maxT.X;
			}

			if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
			{
				if (maxT.Y < 0.0f)
				{
					return null; // ray go on opposite of face
				}

				// coordonate of hit point of face of cube
				var coord = this.Position.Z + maxT.Y * this.Direction.Z;

				// if hit point coord ( intersect face with ray) is out of other plane coord it miss
				if (coord < box.Min.Z || coord > box.Max.Z)
				{
					return null;
				}

				coord = this.Position.X + maxT.Y * this.Direction.X;
				if (coord < box.Min.X || coord > box.Max.X)
				{
					return null;
				}

				return maxT.Y;
			}
			else
			{
				// Z
				if (maxT.Z < 0.0f)
				{
					return null; // ray go on opposite of face
				}

				// coordonate of hit point of face of cube
				var coord = this.Position.X + maxT.Z * this.Direction.X;

				// if hit point coord ( intersect face with ray) is out of other plane coord it miss
				if (coord < box.Min.X || coord > box.Max.X)
				{
					return null;
				}

				coord = this.Position.Y + maxT.Z * this.Direction.Y;
				if (coord < box.Min.Y || coord > box.Max.Y)
				{
					return null;
				}

				return maxT.Z;
			}
		}

		public static bool operator !=(Ray a, Ray b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Ray a, Ray b)
		{
			return a.Equals(b);
		}

		public override string ToString()
		{
			return string.Format("{{Position:{0} Direction:{1}}}", this.Position, this.Direction);
		}

		#endregion
	}
}