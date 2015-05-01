using System.Collections.Generic;

namespace SharpMC.Utils
{
	public class Records : List<Vector3>
	{
		public Records()
		{
		}

		public Records(IEnumerable<Vector3> coordinates) : base(coordinates)
		{
		}
	}
}