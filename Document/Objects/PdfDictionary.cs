using Kwesoft.Pdf.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfDictionary : PdfObject
	{
		public Dictionary<PdfName, PdfObject> Value { get; set; }

		private string _GetValues()
		{
			if (Value.Count == 0) return "";
			var values = string.Join(PdfKeywords.LineBreak, Value.Select(kvp => $"{kvp.Key}{PdfKeywords.Space}{kvp.Value}"));
			return $"{values}";
		}

		public override string ToString()
		{
			var values = _GetValues();
			return $"{PdfKeywords.DictionaryStart}{values}{PdfKeywords.DictionaryEnd}";
		}
	}
}
