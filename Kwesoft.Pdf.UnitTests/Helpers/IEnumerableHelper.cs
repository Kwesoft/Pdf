using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.UnitTests.Helpers
{
	internal static class IEnumerableHelper
	{
		internal static void AssertAllEqual<T>(this IReadOnlyList<T> expected, IReadOnlyList<T> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);
			for (var i = 0; i < expected.Count; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}
	}
}
