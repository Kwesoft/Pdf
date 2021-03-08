using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfString : PdfObject
	{
		public string Value { get; set; }
		public override string ToString() => $"{PdfKeywords.StringStart}{Value}{PdfKeywords.StringEnd}";
	}
}
