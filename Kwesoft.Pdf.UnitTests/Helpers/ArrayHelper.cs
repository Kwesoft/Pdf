using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.UnitTests.Helpers
{
	internal static class ArrayHelper
	{
		internal static void AssertArrayEqual<T>(this T[] expected, T[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length);
			for (var i = 0; i < expected.Length; i++)
				Assert.AreEqual(expected[i], actual[i]);
		}
	}
}
