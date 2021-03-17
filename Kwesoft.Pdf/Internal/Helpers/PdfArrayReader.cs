using Kwesoft.Pdf.Helpers;
using System.Collections.Generic;

namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfArrayReader
	{
		internal static PdfArray ReadArray(this IEditablePdfDocument document, int index, PdfObject parent)
		{
			var pos = index + 1;
			var list = new List<PdfObject>();
			var result = new PdfArray(list)
			{
				Offset = index - (parent?.Index ?? 0),
				Parent = parent,
				Document = document
			};

			while (!document.ByteEquals(pos, document.Keywords.ArrayEnd))
			{
				var obj = document.ReadObject(pos, result);
				pos += obj.Length;
				list.Add(obj);
				while (document.ByteEquals(pos, document.Keywords.Space) || document.ByteEquals(pos, document.Keywords.LineBreak))
					pos++;
			}

			result.Length = pos - index;
			return result;
		}
	}
}
