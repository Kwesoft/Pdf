using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class ByteArrayHelper
	{
		
		internal static void Insert(this byte[] data, byte newValue)
		{
			for (var i = 1; i < data.Length; i++)
			{
				data[i - 1] = data[i];
			}
			data[^1] = newValue;
		}

		internal static void InsertReverse(this byte[] data, byte newValue)
		{
			for (var i = data.Length - 1; i > 0; i--)
			{
				data[i] = data[i - 1];
			}
			data[0] = newValue;
		}

		internal static bool ByteEquals(this byte[] a, byte[] b) => a.ByteEquals(0, b, 0);
		internal static bool ByteEquals(this byte[] a, int posA, byte[] b) => a.ByteEquals(posA, b, 0);
		internal static bool ByteEquals(this byte[] a, int posA, byte[] b, int posB)
		{
			var lengthA = a.Length - posA;
			var lengthB = b.Length - posB;
			var length = lengthA < lengthB ? lengthA : lengthB;
			for (var i = 0; i < length; i++)
			{
				if (a[posA + i] != b[posB + i]) return false;
			}
			return true;
		}
	}
}
