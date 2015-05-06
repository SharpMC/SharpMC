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
	public class OLDVector3
	{
		public OLDVector3(double _X, double _Y, double _Z)
		{
			X = _X;
			Y = _Y;
			Z = _Z;
		}

		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public string GetString()
		{
			return X + ", " + Y + ", " + Z;
		}

		public void ConvertToNetwork()
		{
			X = HostToNetworkOrder(X);
			Y = HostToNetworkOrder(Y);
			Z = HostToNetworkOrder(Z);
		}

		public void ConvertToHost()
		{
			X = NetworkToHostOrder(X);
			Y = NetworkToHostOrder(Y);
			Z = NetworkToHostOrder(Z);
		}

		private double HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private double NetworkToHostOrder(double d)
		{
			var data = BitConverter.GetBytes(d);
			if (!BitConverter.IsLittleEndian)
				Array.Reverse(data);
			return BitConverter.ToDouble(data, 0);
		}

		public double DistanceBetween(OLDVector3 other)
		{
			return Math.Sqrt(Square(other.X - X) +
			                 Square(other.Y - Y) +
			                 Square(other.Z - Z));
		}

		private double Square(double num)
		{
			return num*num;
		}

		public static OLDVector3 operator -(OLDVector3 a, OLDVector3 b)
		{
			return new OLDVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static OLDVector3 operator +(OLDVector3 a, OLDVector3 b)
		{
			return new OLDVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public PlayerLocation ToPlayerLocation()
		{
			return new PlayerLocation(X, Y, Z);
		}
	}
}