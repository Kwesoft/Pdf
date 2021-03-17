using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class StringHelper
	{
		public static byte[] ToByteArray(this string input, int fromBase)
		{
			int NumberChars = input.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), fromBase);
			return bytes;
		}
	}
}
