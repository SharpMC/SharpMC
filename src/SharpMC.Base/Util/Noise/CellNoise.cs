/*
 * Copyright (c) 2011 Richard Klafter, Eric Swanson
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Numerics;

namespace SharpMC.Util.Noise
{
	public class CellNoise : NoiseGen
	{
		public int Seed { get; set; }
		public DistanceType DistanceFunction { get; set; }
		public CombinationFunctions CombinationFunction { get; set; }

		public CellNoise() : this(new Random().Next())
		{
		}

		public CellNoise(int seed)
		{
			Seed = seed;
			DistanceFunction = DistanceType.Euclidean3D;
			CombinationFunction = CombinationFunctions.D2Minusd1;
		}

		public override double Value2D(double x, double y)
		{
			var distances = new double[3];
			//Declare some values for later use
			uint lastRandom, numberFeaturePoints;
			Vector3 randomDiff, featurePoint;

			int cubeX, cubeY;
			//Initialize values in distance array to large values
			for (var i = 0; i < distances.Length; i++)
				distances[i] = 6666;
			//1. Determine which cube the evaluation point is in
			var evalCubeX = Floor(x);
			var evalCubeY = Floor(y);
			for (var i = -1; i < 2; ++i)
			{
				for (var j = -1; j < 2; ++j)
				{
					cubeX = evalCubeX + i;
					cubeY = evalCubeY + j;
					//2. Generate a reproducible random number generator for the cube
					lastRandom = LcgRandom(Hash2D((uint) (cubeX + Seed), (uint) cubeY));
					//3. Determine how many feature points are in the cube
					numberFeaturePoints = ProbLookup(lastRandom);
					//4. Randomly place the feature points in the cube
					for (uint l = 0; l < numberFeaturePoints; ++l)
					{
						lastRandom = LcgRandom(lastRandom);
						randomDiff.X = lastRandom/0x100000000;
						lastRandom = LcgRandom(lastRandom);
						randomDiff.Y = lastRandom/0x100000000;
						lastRandom = LcgRandom(lastRandom);
						randomDiff.Z = lastRandom/0x100000000;
						featurePoint = new Vector3(randomDiff.X + cubeX, randomDiff.Y + cubeY, 0);
						//5. Find the feature point closest to the evaluation point.
						//This is done by inserting the distances to the feature points into a sorted list
						Insert(distances, Distance(new Vector3((float)x, (float)y, 0), featurePoint));
					}
				}
			}
			return Combine(distances);
		}

		/// <summary>
		/// Generates 3D Cell Noise
		/// </summary>
		/// <returns>The color worley noise returns at the input position</returns>
		public override double Value3D(double x, double y, double z)
		{
			var distances = new double[3];
			//Declare some values for later use
			uint lastRandom, numberFeaturePoints;
			Vector3 randomDiff, featurePoint;
			int cubeX, cubeY, cubeZ;
			//Initialize values in distance array to large values
			for (var i = 0; i < distances.Length; i++)
				distances[i] = 6666;
			//1. Determine which cube the evaluation point is in
			var evalCubeX = Floor(x);
			var evalCubeY = Floor(y);
			var evalCubeZ = Floor(z);
			for (var i = -1; i < 2; ++i)
			{
				for (var j = -1; j < 2; ++j)
				{
					for (var k = -1; k < 2; ++k)
					{
						cubeX = evalCubeX + i;
						cubeY = evalCubeY + j;
						cubeZ = evalCubeZ + k;
						//2. Generate a reproducible random number generator for the cube
						lastRandom = LcgRandom(Hash((uint) (cubeX + Seed), (uint) cubeY, (uint) cubeZ));
						//3. Determine how many feature points are in the cube
						numberFeaturePoints = ProbLookup(lastRandom);
						//4. Randomly place the feature points in the cube
						for (uint l = 0; l < numberFeaturePoints; ++l)
						{
							lastRandom = LcgRandom(lastRandom);
							randomDiff.X = lastRandom/0x100000000;
							lastRandom = LcgRandom(lastRandom);
							randomDiff.Y = lastRandom/0x100000000;
							lastRandom = LcgRandom(lastRandom);
							randomDiff.Z = lastRandom/0x100000000;
							featurePoint = new Vector3(randomDiff.X + cubeX, randomDiff.Y + cubeY, randomDiff.Z + cubeZ);
							//5. Find the feature point closest to the evaluation point.
							//This is done by inserting the distances to the feature points into a sorted list
							Insert(distances, Distance(new Vector3((float)x, (float)y, (float)z), featurePoint));
						}
						//6. Check the neighboring cubes to ensure their are no closer evaluation points.
						// This is done by repeating steps 1 through 5 above for each neighboring cube
					}
				}
			}
			return Combine(distances);
		}

		private double Combine(double[] array)
		{
			switch (CombinationFunction)
			{
				case CombinationFunctions.D1:
					return array[0];
				case CombinationFunctions.D2Minusd1:
					return array[1] - array[0];
				case CombinationFunctions.D3Minusd1:
					return array[2] - array[0];
				default:
					return array[0];
			}
		}

		private float Distance(Vector3 p1, Vector3 p2)
		{
			switch (DistanceFunction)
			{
				case DistanceType.Euclidean2D:
					return (float) EuclidianDistance2D(p1, p2);
				case DistanceType.Euclidean3D:
					return (float) EuclidianDistance3D(p1, p2);
				case DistanceType.Chebyshev2D:
					return (float) ChebyshevDistance2D(p1, p2);
				case DistanceType.Chebyshev3D:
					return (float) ChebyshevDistance3D(p1, p2);
				case DistanceType.Manhattan2D:
					return (float) ManhattanDistance2D(p1, p2);
				case DistanceType.Manhattan3D:
					return (float) ManhattanDistance3D(p1, p2);
				default:
					return (float) EuclidianDistance3D(p1, p2);
			}
		}

		private double EuclidianDistance2D(Vector3 p1, Vector3 p2)
		{
			return (p1.X - p2.X)*(p1.X - p2.X) + (p1.Y - p2.Y)*(p1.Y - p2.Y);
		}

		private double EuclidianDistance3D(Vector3 p1, Vector3 p2)
		{
			return (p1.X - p2.X)*(p1.X - p2.X) + (p1.Y - p2.Y)*(p1.Y - p2.Y) + (p1.Z - p2.Z)*(p1.Z - p2.Z);
		}

		private double ManhattanDistance2D(Vector3 p1, Vector3 p2)
		{
			return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
		}

		private double ManhattanDistance3D(Vector3 p1, Vector3 p2)
		{
			return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
		}

		private double ChebyshevDistance2D(Vector3 p1, Vector3 p2)
		{
			var diff = p1 - p2;
			return Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
		}

		private double ChebyshevDistance3D(Vector3 p1, Vector3 p2)
		{
			var diff = p1 - p2;
			return Math.Max(Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)), Math.Abs(diff.Z));
		}

		/// <summary>
		/// Given a uniformly distributed random number this function returns the number of feature points in a given cube.
		/// </summary>
		/// <param name="value">a uniformly distributed random number</param>
		/// <returns>The number of feature points in a cube.</returns>
		// Generated using mathmatica with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]"
		private static uint ProbLookup(uint value)
		{
			if (value < 393325350)
				return 1;
			if (value < 1022645910)
				return 2;
			if (value < 1861739990)
				return 3;
			if (value < 2700834071)
				return 4;
			if (value < 3372109335)
				return 5;
			if (value < 3819626178)
				return 6;
			if (value < 4075350088)
				return 7;
			if (value < 4203212043)
				return 8;
			return 9;
		}

		/// <summary>
		/// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
		/// it will not be added to the array.
		/// </summary>
		/// <param name="arr">The array to insert the value into.</param>
		/// <param name="value">The value to insert into the array.</param>
		private static void Insert(double[] arr, double value)
		{
			double temp;
			for (var i = arr.Length - 1; i >= 0; i--)
			{
				if (value > arr[i])
					break;
				temp = arr[i];
				arr[i] = value;
				if (i + 1 < arr.Length)
					arr[i + 1] = temp;
			}
		}

		/// <summary>
		/// LCG Random Number Generator.
		/// LCG: http://en.wikipedia.org/wiki/Linear_congruential_generator
		/// </summary>
		/// <param name="lastValue">The last value calculated by the lcg or a seed</param>
		/// <returns>A new random number</returns>
		private static uint LcgRandom(uint lastValue)
		{
			return (uint) ((1103515245u*lastValue + 12345u)%0x100000000u);
		}

		/// <summary>
		/// Constant used in FNV hash function.
		/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
		/// </summary>
		private const uint OffsetBasis = 2166136261;

		/// <summary>
		/// Constant used in FNV hash function
		/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
		/// </summary>
		private const uint FnvPrime = 16777619;

		/// <summary>
		/// Hashes three integers into a single integer using FNV hash.
		/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
		/// </summary>
		/// <returns>hash value</returns>
		private static uint Hash(uint i, uint j, uint k)
		{
			return (uint) ((((((OffsetBasis ^ (uint) i)*FnvPrime) ^ (uint) j)*FnvPrime) ^ (uint) k)*FnvPrime);
		}

		/// <summary>
		/// Hashes three integers into a single integer using FNV hash.
		/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
		/// </summary>
		/// <returns>hash value</returns>
		private static uint Hash2D(uint i, uint j)
		{
			return (uint) ((((OffsetBasis ^ (uint) i)*FnvPrime) ^ (uint) j)*FnvPrime);
		}
	}

	public enum CombinationFunctions
	{
		D1,
		D2Minusd1,
		D3Minusd1
	}

	public enum DistanceType
	{
		Euclidean3D,
		Euclidean2D,
		Manhattan3D,
		Manhattan2D,
		Chebyshev3D,
		Chebyshev2D
	}
}