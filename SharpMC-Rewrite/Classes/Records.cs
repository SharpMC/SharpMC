using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Classes
{
	public class Records : List<INTVector3>
	{
		public Records()
		{
		}

		public Records(IEnumerable<INTVector3> coordinates) : base(coordinates)
		{
		}
	}
}
