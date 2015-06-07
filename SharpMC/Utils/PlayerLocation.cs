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

	public class PlayerLocation
	{
		public PlayerLocation(double _X, double _Y, double _Z)
		{
			this.X = _X;
			this.Y = _Y;
			this.Z = _Z;
		}

		public float Yaw { get; set; }

		public float Pitch { get; set; }

		public bool OnGround { get; set; }

		public double X { get; set; }

		public double Y { get; set; }

		public double Z { get; set; }

		public Vector3 ToVector3()
		{
			return new Vector3(this.X, this.Y, this.Z);
		}

		public double DistanceTo(PlayerLocation other)
		{
			return Math.Sqrt(this.Square(other.X - this.X) + this.Square(other.Y - this.Y) + this.Square(other.Z - this.Z));
		}

		private double Square(double num)
		{
			return num * num;
		}
	}
}