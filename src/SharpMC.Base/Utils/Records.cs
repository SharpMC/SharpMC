using System.Collections.Generic;
using System.Numerics;

namespace SharpMC.Core.Utils
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