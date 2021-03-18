using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class IEditablePdfDocumentHelper
	{
		internal static bool ByteEquals(this IEditablePdfDocument a, byte[] b) => a.ByteEquals(0, b, 0);
		internal static bool ByteEquals(this IEditablePdfDocument a, int posA, byte[] b) => a.ByteEquals(posA, b, 0);
		internal static bool ByteEquals(this IEditablePdfDocument a, int posA, byte[] b, int posB) => a.Read(posA, b.Length - posB).ByteEquals(0, b, posB);

		internal static int Find(this IEditablePdfDocument document, byte[] lookFor, int start) => document.Find(lookFor.Length, w => w.ByteEquals(lookFor), start);
		internal static int Find(this IEditablePdfDocument document, byte[] lookFor) => document.Find(lookFor, 0);
		internal static int Find(this IEditablePdfDocument document, int windowSize, Func<byte[], bool> matches, int start = 0)
		{
			var end = document.Length;

			var pos = start;
			var window = new byte[windowSize];
			while (pos <= end)
			{
				window.Insert(document.Read(pos));
				if (matches(window)) return pos;
				pos++;
			}
			return -1;
		}
		internal static int FindReverse(this IEditablePdfDocument document, int windowSize, Func<byte[], bool> matches) => document.FindReverse(windowSize, matches, document.Length - 1);
		internal static int FindReverse(this IEditablePdfDocument document, byte[] lookFor, int start) => document.FindReverse(lookFor.Length, w => w.ByteEquals(lookFor), start);
		internal static int FindReverse(this IEditablePdfDocument document, byte[] lookFor) => document.FindReverse(lookFor, document.Length - 1);
		internal static int FindReverse(this IEditablePdfDocument document, int windowSize, Func<byte[], bool> matches, int start)
		{
			var end = 0;

			var pos = start;
			var window = new byte[windowSize];
			while (pos != end)
			{
				window.InsertReverse(document.Read(pos));
				if (matches(window)) return pos;
				pos--;
			}
			return -1;
		}
	}
}
