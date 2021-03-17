using System;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfStringReader
	{
		internal static PdfString ReadString(this IEditablePdfDocument document, int index, PdfObject parent) => document._ReadString(index, parent, _ReadSanitisedString);
		internal static PdfString ReadHexString(this IEditablePdfDocument document, int index, PdfObject parent) => document._ReadString(index, parent, _ReadHexSanitisedString);


		private static PdfString _ReadString(this IEditablePdfDocument document, int index, PdfObject parent, Func<IEditablePdfDocument, string, string> readValue)
		{
			var length = document.Find(document.Keywords.StringEnd, index) - index;
			return new PdfString
			{
				Offset = index - (parent?.Index ?? 0),
				Length = length + 1,
				Value = readValue(document, document.Encoding.GetString(document.Read(index + 1, length - 1))),
				Parent = parent,
				Document = document
			};
		}


		private static string _ReadSanitisedString(IEditablePdfDocument document, string input)
		{
			var output = "";
			var i = 0;

			while (i < input.Length)
			{
				if (input[i] == '\\')
				{
					output = $"{output}{document.Encoding.GetString(input.Substring(i + 1, 3).ToByteArray(8))}";
					i += 4;
				}
				else
				{
					var e = input.IndexOf('\\', i);
					e = e == -1 ? input.Length : e;
					output = $"{output}{input.Substring(i, (e - i) - 1)}";
					i = e;
				}
			}

			return new string(output.ToArray());
		}
		private static string _ReadHexSanitisedString(IEditablePdfDocument document, string input)
		{
			if (input.Substring(0, 4).ToUpper() == "FFFE")
				return Encoding.BigEndianUnicode.GetString(input.Substring(4, input.Length - 4).ToByteArray(16));
			return document.Encoding.GetString(input.ToByteArray(16));
		}
	}
}
