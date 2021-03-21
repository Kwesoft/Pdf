using System;
using System.Collections.Generic;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class StringHelper
	{
		public static byte[] HexToByteArray(this string input)
		{
			int NumberChars = input.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
			return bytes;
		}

		public static byte[] OctToByteArray(this string input)
		{
			int NumberChars = input.Length;
			byte[] bytes = new byte[NumberChars / 3];
			for (int i = 0; i < NumberChars; i += 3)
				bytes[i / 3] = Convert.ToByte(input.Substring(i, 3), 8);
			return bytes;
		}
	}
}
