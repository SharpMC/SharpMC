using System.Collections.Generic;

namespace SharpMCRewrite.Classes
{
	public class Records : List<IntVector3>
	{
		public Records()
		{
		}

		public Records(IEnumerable<IntVector3> coordinates) : base(coordinates)
		{
		}
	}
}