using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class HashCodeHelper
	{
		internal static int Combine(object o1, object o2)
		{
			var h1 = o1.GetHashCode();
			var h2 = o2.GetHashCode();
			return (((h1 << 5) + h1) ^ h2);
		}
	}
}
