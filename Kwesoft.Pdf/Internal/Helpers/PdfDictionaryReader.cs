using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;

namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfDictionaryReader
	{
		internal static PdfDictionary ReadDictionary(this IEditablePdfDocument document, int index, PdfObject parent)
		{
			if (!document.ByteEquals(index, document.Keywords.DictionaryStart)) throw new Exception("Invalid dictionary");
			var pos = index + 2;
			var dictionary = new Dictionary<PdfName, PdfObject>();
			var result = new PdfDictionary(dictionary)
			{
				Offset = index - (parent?.Index ?? 0),
				Parent = parent,
				Document = document
			};

			while (!document.ByteEquals(pos, document.Keywords.DictionaryEnd))
			{
				if (document.ByteEquals(pos, document.Keywords.NameStart))
				{
					var key = document.ReadName(pos, result);
					pos += key.Length + 1;
					var obj = document.ReadObject(pos, result);
					dictionary.Add(key, obj);
					pos += obj.Length;
				}
				else
				{
					pos++;
				}
			}
			result.Length = (pos + 2) - index;
			return result;
		}
	}
}
