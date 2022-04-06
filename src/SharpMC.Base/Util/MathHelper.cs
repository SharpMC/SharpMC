using System;

namespace SharpMC.Util
{
	public class MathHelper
	{

		private static float[] a = new float[65536];
		private static int[] b;

		public static float sin(float f)
		{
			return a[(int)(f * 10430.378F) & '\uffff'];
		}

		public static float cos(float f)
		{
			return a[(int)(f * 10430.378F + 16384.0F) & '\uffff'];
		}

		public static float c(float f)
		{
			return (float)Math.Sqrt((double)f);
		}

		public static float sqrt(double d0)
		{
			return (float)Math.Sqrt(d0);
		}

		public static int d(float f)
		{
			int i = (int)f;

			return f < (float)i ? i - 1 : i;
		}

		public static int floor(double d0)
		{
			int i = (int)d0;

			return d0 < (double)i ? i - 1 : i;
		}

		public static long d(double d0)
		{
			long i = (long)d0;

			return d0 < (double)i ? i - 1L : i;
		}

		public static float abs(float f)
		{
			return f >= 0.0F ? f : -f;
		}

		public static int A(int i)
		{
			return i >= 0 ? i : -i;
		}

		public static int f(float f)
		{
			int i = (int)f;

			return f > (float)i ? i + 1 : i;
		}

		public static int f(double d0)
		{
			int i = (int)d0;

			return d0 > (double)i ? i + 1 : i;
		}

		public static int A(int i, int j, int k)
		{
			return i < j ? j : (i > k ? k : i);
		}

		public static float A(float f, float f1, float f2)
		{
			return f < f1 ? f1 : (f > f2 ? f2 : f);
		}

		public static double A(double d0, double d1, double d2)
		{
			return d0 < d1 ? d1 : (d0 > d2 ? d2 : d0);
		}

		public static double B(double d0, double d1, double d2)
		{
			return d2 < 0.0D ? d0 : (d2 > 1.0D ? d1 : d0 + (d1 - d0) * d2);
		}

		public static double A(double d0, double d1)
		{
			if (d0 < 0.0D)
			{
				d0 = -d0;
			}

			if (d1 < 0.0D)
			{
				d1 = -d1;
			}

			return d0 > d1 ? d0 : d1;
		}

		public static int nextInt(Random random, int i, int j)
		{
			return i >= j ? i : random.Next(j - i + 1) + i;
		}

		public static float A(Random random, float f, float f1)
		{
			return f >= f1 ? f : (float)(random.NextDouble()) * (f1 - f) + f;
		}

		public static double A(Random random, double d0, double d1)
		{
			return d0 >= d1 ? d0 : random.NextDouble() * (d1 - d0) + d0;
		}

		public static double A(long[] along)
		{
			long i = 0L;
			long[] along1 = along;
			int j = along.Length;

			for (int k = 0; k < j; ++k)
			{
				long l = along1[k];

				i += l;
			}

			return (double)i / (double)along.Length;
		}

		public static float g(float f)
		{
			f %= 360.0F;
			if (f >= 180.0F)
			{
				f -= 360.0F;
			}

			if (f < -180.0F)
			{
				f += 360.0F;
			}

			return f;
		}

		public static double g(double d0)
		{
			d0 %= 360.0D;
			if (d0 >= 180.0D)
			{
				d0 -= 360.0D;
			}

			if (d0 < -180.0D)
			{
				d0 += 360.0D;
			}

			return d0;
		}

		public static int A(String s, int i)
		{
			int j = i;

			try
			{
				j = int.Parse(s);
			}
			catch
			{
				;
			}

			return j;
		}

		public static int A(String s, int i, int j)
		{
			int k = i;

			try
			{
				k = int.Parse(s);
			}
			catch
			{
				
			}

			if (k < j)
			{
				k = j;
			}

			return k;
		}

		public static double A(String s, double d0)
		{
			double d1 = d0;

			try
			{
				d1 = double.Parse(s);
			}
			catch
			{
				;
			}

			return d1;
		}

		public static double A(String s, double d0, double d1)
		{
			double d2 = d0;

			try
			{
				d2 = double.Parse(s);
			}
			catch
			{
				;
			}

			if (d2 < d1)
			{
				d2 = d1;
			}

			return d2;
		}

		static MathHelper()
		{
			for (int i = 0; i < 65536; ++i)
			{
				a[i] = (float) Math.Sin((double) i*3.141592653589793D*2.0D/65536.0D);
			}

			b = new int[]
			{
				0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
			};
		}
	}
}
