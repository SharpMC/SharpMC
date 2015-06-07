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
	using System.Collections.Generic;

	using SharpMC.Enums;

	public struct BoundingBox : IEquatable<BoundingBox>
	{
		public const int CornerCount = 8;

		public Vector3 Max;

		public Vector3 Min;

		public BoundingBox(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		public BoundingBox(BoundingBox box)
		{
			this.Min = new Vector3(box.Min);
			this.Max = new Vector3(box.Max);
		}

		public double Height
		{
			get
			{
				return this.Max.Y - this.Min.Y;
			}
		}

		public double Width
		{
			get
			{
				return this.Max.X - this.Min.X;
			}
		}

		public double Depth
		{
			get
			{
				return this.Max.Z - this.Min.Z;
			}
		}

		public bool Equals(BoundingBox other)
		{
			return (this.Min == other.Min) && (this.Max == other.Max);
		}

		public ContainmentType Contains(BoundingBox box)
		{
			// test if all corner is in the same side of a face by just checking min and max
			if (box.Max.X < this.Min.X || box.Min.X > this.Max.X || box.Max.Y < this.Min.Y || box.Min.Y > this.Max.Y
			    || box.Max.Z < this.Min.Z || box.Min.Z > this.Max.Z)
			{
				return ContainmentType.Disjoint;
			}

			if (box.Min.X >= this.Min.X && box.Max.X <= this.Max.X && box.Min.Y >= this.Min.Y && box.Max.Y <= this.Max.Y
			    && box.Min.Z >= this.Min.Z && box.Max.Z <= this.Max.Z)
			{
				return ContainmentType.Contains;
			}

			return ContainmentType.Intersects;
		}

		public bool Contains(Vector3 vec)
		{
			return this.Min.X <= vec.X && vec.X <= this.Max.X && this.Min.Y <= vec.Y && vec.Y <= this.Max.Y
			       && this.Min.Z <= vec.Z && vec.Z <= this.Max.Z;
		}

		public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException();
			}

			var empty = true;
			var vector2 = new Vector3(float.MaxValue);
			var vector1 = new Vector3(float.MinValue);
			foreach (var vector3 in points)
			{
				vector2 = Vector3.Min(vector2, vector3);
				vector1 = Vector3.Max(vector1, vector3);
				empty = false;
			}

			if (empty)
			{
				throw new ArgumentException();
			}

			return new BoundingBox(vector2, vector1);
		}

		public BoundingBox OffsetBy(Vector3 offset)
		{
			return new BoundingBox(this.Min + offset, this.Max + offset);
		}

		public Vector3[] GetCorners()
		{
			return new[]
				       {
					       new Vector3(this.Min.X, this.Max.Y, this.Max.Z), new Vector3(this.Max.X, this.Max.Y, this.Max.Z), 
					       new Vector3(this.Max.X, this.Min.Y, this.Max.Z), new Vector3(this.Min.X, this.Min.Y, this.Max.Z), 
					       new Vector3(this.Min.X, this.Max.Y, this.Min.Z), new Vector3(this.Max.X, this.Max.Y, this.Min.Z), 
					       new Vector3(this.Max.X, this.Min.Y, this.Min.Z), new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
				       };
		}

		public override bool Equals(object obj)
		{
			return (obj is BoundingBox) && this.Equals((BoundingBox)obj);
		}

		public override int GetHashCode()
		{
			return this.Min.GetHashCode() + this.Max.GetHashCode();
		}

		public bool Intersects(BoundingBox box)
		{
			bool result;
			this.Intersects(ref box, out result);
			return result;
		}

		public void Intersects(ref BoundingBox box, out bool result)
		{
			if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
			{
				if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
				{
					result = false;
					return;
				}

				result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
				return;
			}

			result = false;
		}

		public static BoundingBox operator +(BoundingBox a, double b)
		{
			return new BoundingBox(a.Min - b, a.Max + b);
		}

		public static bool operator ==(BoundingBox a, BoundingBox b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BoundingBox a, BoundingBox b)
		{
			return !a.Equals(b);
		}

		public override string ToString()
		{
			return string.Format("{{Min:{0} Max:{1}}}", this.Min, this.Max);
		}
	}
}