using System;
using System.Linq;

namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfNameReader
	{
		private static string _ReadSanitisedName(string input)
		{
			var output = "";
			var i = 0;

			while (i < input.Length)
			{
				if (input[i] == '#')
				{
					output = $"{output}{(char)Convert.ToByte(input.Substring(i + 1, 2), 16)}";
					i += 3;
				}
				else if (input[i] == '/')
				{
					output = $"{output}{input[i + 1]}";
					i += 2;
				}
				else
				{
					var e1 = input.IndexOf('#', i);
					var e2 = input.IndexOf('/', i);
					e1 = e1 == -1 ? input.Length : e1;
					e2 = e2 == -1 ? input.Length : e2;
					var e = e1 < e2 ? e1 : e2;
					output = $"{output}{input.Substring(i, (e - i) - 1)}";
					i = e;
				}
			}

			return new string(output.ToArray());
		}


		internal static PdfName ReadName(this IEditablePdfDocument document, int index, PdfObject parent)
		{
			var end = document.Find(1, b => b.ByteEquals(document.Keywords.Space) || b.ByteEquals(document.Keywords.ArrayEnd) || b.ByteEquals(document.Keywords.DictionaryEnd) || b.ByteEquals(document.Keywords.LineBreak), index + 1);
			return new PdfName
			{
				Value = _ReadSanitisedName(document.Encoding.GetString(document.Read(index + 1, end - (index + 1)))),
				Offset = index - (parent?.Index ?? 0),
				Length = end - index,
				Parent = parent,
				Document = document
			};
		}
	}
}
