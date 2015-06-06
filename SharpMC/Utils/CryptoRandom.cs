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
using System.Security.Cryptography;

namespace SharpMC.Utils
{
	public class CryptoRandom : RandomNumberGenerator
	{
		private static RandomNumberGenerator r;

		/// <summary>
		///     Creates an instance of the default implementation of a cryptographic random number generator that can be used to
		///     generate random data.
		/// </summary>
		public CryptoRandom()
		{
			r = Create();
		}

		/// <summary>
		///     Fills the elements of a specified array of bytes with random numbers.
		/// </summary>
		/// <param name=” buffer”>An array of bytes to contain random numbers.</param>
		public override void GetBytes(byte[] buffer)
		{
			r.GetBytes(buffer);
		}

		/// <summary>
		///     Returns a random number between 0.0 and 1.0.
		/// </summary>
		public double NextDouble()
		{
			var b = new byte[4];
			r.GetBytes(b);
			return (double) BitConverter.ToUInt32(b, 0)/uint.MaxValue;
		}

		/// <summary>
		///     Returns a random number within the specified range.
		/// </summary>
		/// <param name=” minValue”>The inclusive lower bound of the random number returned.</param>
		/// <param name=” maxValue”>
		///     The exclusive upper bound of the random number returned. maxValue must be greater than or equal
		///     to minValue.
		/// </param>
		public int Next(int minValue, int maxValue)
		{
			return (int) Math.Round(NextDouble()*(maxValue - minValue - 1)) + minValue;
		}

		/// <summary>
		///     Returns a nonnegative random number.
		/// </summary>
		public int Next()
		{
			return Next(0, int.MaxValue);
		}

		/// <summary>
		///     Returns a nonnegative random number less than the specified maximum
		/// </summary>
		/// <param name=” maxValue”>
		///     The inclusive upper bound of the random number returned. maxValue must be greater than or equal
		///     0
		/// </param>
		public int Next(int maxValue)
		{
			return Next(0, maxValue);
		}
	}
}