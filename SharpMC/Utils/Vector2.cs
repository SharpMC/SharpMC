namespace SharpMC.Utils
{
	public class Vector2
	{
		public Vector2(int _X, int _Z)
		{
			X = _X;
			Z = _Z;
		}

		public int X { get; set; }
		public int Z { get; set; }

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Z - b.Z);
		}
	}
}