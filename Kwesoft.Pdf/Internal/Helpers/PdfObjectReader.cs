using Kwesoft.Pdf.Helpers;
using System.Linq;

namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfObjectReader
	{
		internal static PdfObject ReadObject(this IEditablePdfDocument document, int index, PdfObject parent)
		{
			if (document.ByteEquals(index, document.Keywords.True)) return new PdfBool { Value = true, Offset = index - (parent?.Index ?? 0), Length = document.Keywords.True.Length, Parent = parent };
			if (document.ByteEquals(index, document.Keywords.False)) return new PdfBool { Value = false, Offset = index - (parent?.Index ?? 0), Length = document.Keywords.False.Length, Parent = parent };
			if (document.ByteEquals(index, document.Keywords.Null)) return new PdfNull { Offset = index - (parent?.Index ?? 0), Length = document.Keywords.Null.Length, Parent = parent };
			if (document.ByteEquals(index, document.Keywords.DictionaryStart)) return document.ReadDictionary(index, parent);
			if (document.ByteEquals(index, document.Keywords.StringStart)) return document.ReadString(index, parent);
			if (document.ByteEquals(index, document.Keywords.HexStringStart)) return document.ReadHexString(index, parent);
			if (document.ByteEquals(index, document.Keywords.ArrayStart)) return document.ReadArray(index, parent);
			if (document.ByteEquals(index, document.Keywords.NameStart)) return document.ReadName(index, parent);

			var pos = index;
			var isDecimal = false;

			while (true)
			{
				if (document.ByteEquals(pos, document.Keywords.Decimal))
				{
					isDecimal = true;
				}
				else if (document.ByteEquals(pos, document.Keywords.DictionaryEnd) || document.ByteEquals(pos, document.Keywords.LineBreak) || document.ByteEquals(pos, document.Keywords.ArrayEnd))
				{
					if (isDecimal)
						return document.ReadDouble(index, pos - index, parent);
					return document.ReadInteger(index, pos - index, parent);
				}
				else if (document.ByteEquals(pos, document.Keywords.Space))
				{
					if (isDecimal)
						return document.ReadDouble(index, pos - index, parent);

					var i = 1;
					while (_IsNumeric(document, pos + i))
						i++;

					if (document.ByteEquals(pos + i, document.Keywords.Space) && document.ByteEquals(pos + i + 1, document.Keywords.Reference))
						return document.ReadIndirectReference(index, (pos + i + 2) - index, parent);

					return document.ReadInteger(index, pos - index, parent);
				}
				pos++;
			}
		}

		private static bool _IsNumeric(IEditablePdfDocument document, int index)
		{
			return document.Keywords.Numbers.Contains(document.Read(index));
		}
	}
}
