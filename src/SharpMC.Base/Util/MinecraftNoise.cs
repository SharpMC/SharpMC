using System;

namespace SharpMC.Util
{
	public class MinecraftNoise
	{
		private static readonly double[] Gradients1 = { 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 0.0D, -1.0D, 0.0D };

		private static readonly double[] Gradients2 = { 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D };

		private static readonly double[] Gradients3 = { 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 1.0D, -1.0D, -1.0D, 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 1.0D, 0.0D, -1.0D };

		private static readonly double[] Gradients4 = { 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 1.0D, -1.0D, 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 0.0D, -1.0D, 0.0D };

		private static readonly double[] Gradients5 = { 0.0D, 0.0D, 0.0D, 0.0D, 1.0D, 1.0D, -1.0D, -1.0D, 1.0D, 1.0D, -1.0D, -1.0D, 0.0D, 1.0D, 0.0D, -1.0D };

		public double A;
		public double B;
		public double C;
		private readonly int[] _d;

		public MinecraftNoise() : this(new Random())
		{
		}

		public MinecraftNoise(Random random)
		{
			_d = new int[512];
			A = random.NextDouble() * 256.0D;
			B = random.NextDouble() * 256.0D;
			C = random.NextDouble() * 256.0D;

			int i;

			for (i = 0; i < 256; _d[i] = i++)
			{
			}

			for (i = 0; i < 256; ++i)
			{
				var j = random.Next(256 - i) + i;
				var k = _d[i];

				_d[i] = _d[j];
				_d[j] = k;
				_d[i + 256] = _d[i];
			}
		}

		public double Noise(double d0, double d1, double d2)
		{
			return d1 + d0 * (d2 - d1);
		}

		public double Noise(int index, double d0, double d1)
		{
			var j = index & 15;

			return Gradients4[j] * d0 + Gradients5[j] * d1;
		}

		public double Noise(int i, double d0, double d1, double d2)
		{
			var j = i & 15;

			return Gradients1[j] * d0 + Gradients2[j] * d1 + Gradients3[j] * d2;
		}

		public void Noise(double[] adouble, double d0, double d1, double d2, int i, int j, int k, double d3, double d4,
			double d5, double d6)
		{
			int l;
			int i1;
			double d7;
			double d8;
			double d9;
			int j1;
			double d10;
			int k1;
			int l1;
			int i2;
			int j2;

			if (j == 1)
			{
				j2 = 0;
				var d13 = 1.0D / d6;

				for (var k2 = 0; k2 < i; ++k2)
				{
					d7 = d0 + k2 * d3 + A;
					var l2 = (int)d7;

					if (d7 < l2)
					{
						--l2;
					}

					var i3 = l2 & 255;

					d7 -= l2;
					d8 = d7 * d7 * d7 * (d7 * (d7 * 6.0D - 15.0D) + 10.0D);

					for (j1 = 0; j1 < k; ++j1)
					{
						d9 = d2 + j1 * d5 + C;
						k1 = (int)d9;
						if (d9 < k1)
						{
							--k1;
						}

						l1 = k1 & 255;
						d9 -= k1;
						d10 = d9 * d9 * d9 * (d9 * (d9 * 6.0D - 15.0D) + 10.0D);
						l = _d[i3] + 0;
						var j3 = _d[l] + l1;
						var k3 = _d[i3 + 1] + 0;

						i1 = _d[k3] + l1;
						var d11 = Noise(d8, Noise(_d[j3], d7, d9), Noise(_d[i1], d7 - 1.0D, 0.0D, d9));
						var d12 = Noise(d8, Noise(_d[j3 + 1], d7, 0.0D, d9 - 1.0D),
							Noise(_d[i1 + 1], d7 - 1.0D, 0.0D, d9 - 1.0D));
						var d14 = Noise(d10, d11, d12);

						i2 = j2++;
						adouble[i2] += d14 * d13;
					}
				}
			}
			else
			{
				l = 0;
				var d15 = 1.0D / d6;

				i1 = -1;
				var d16 = 0.0D;

				d7 = 0.0D;
				var d17 = 0.0D;

				d8 = 0.0D;

				for (j1 = 0; j1 < i; ++j1)
				{
					d9 = d0 + j1 * d3 + A;
					k1 = (int)d9;
					if (d9 < k1)
					{
						--k1;
					}

					l1 = k1 & 255;
					d9 -= k1;
					d10 = d9 * d9 * d9 * (d9 * (d9 * 6.0D - 15.0D) + 10.0D);

					for (var l3 = 0; l3 < k; ++l3)
					{
						var d18 = d2 + l3 * d5 + C;
						var i4 = (int)d18;

						if (d18 < i4)
						{
							--i4;
						}

						var j4 = i4 & 255;

						d18 -= i4;
						var d19 = d18 * d18 * d18 * (d18 * (d18 * 6.0D - 15.0D) + 10.0D);

						for (var k4 = 0; k4 < j; ++k4)
						{
							var d20 = d1 + k4 * d4 + B;
							var l4 = (int)d20;

							if (d20 < l4)
							{
								--l4;
							}

							var i5 = l4 & 255;

							d20 -= l4;
							var d21 = d20 * d20 * d20 * (d20 * (d20 * 6.0D - 15.0D) + 10.0D);

							if (k4 == 0 || i5 != i1)
							{
								i1 = i5;
								var j5 = _d[l1] + i5;
								var k5 = _d[j5] + j4;
								var l5 = _d[j5 + 1] + j4;
								var i6 = _d[l1 + 1] + i5;

								j2 = _d[i6] + j4;
								var j6 = _d[i6 + 1] + j4;

								d16 = Noise(d10, Noise(_d[k5], d9, d20, d18),
									Noise(_d[j2], d9 - 1.0D, d20, d18));
								d7 = Noise(d10, Noise(_d[l5], d9, d20 - 1.0D, d18),
									Noise(_d[j6], d9 - 1.0D, d20 - 1.0D, d18));
								d17 = Noise(d10, Noise(_d[k5 + 1], d9, d20, d18 - 1.0D),
									Noise(_d[j2 + 1], d9 - 1.0D, d20, d18 - 1.0D));
								d8 = Noise(d10, Noise(_d[l5 + 1], d9, d20 - 1.0D, d18 - 1.0D),
									Noise(_d[j6 + 1], d9 - 1.0D, d20 - 1.0D, d18 - 1.0D));
							}

							var d22 = Noise(d21, d16, d7);
							var d23 = Noise(d21, d17, d8);
							var d24 = Noise(d19, d22, d23);

							i2 = l++;
							adouble[i2] += d24 * d15;
						}
					}
				}
			}
		}
	}
}
