using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfString : PdfObject<string>
	{ 
		public override string ToString() => $"{PdfKeywords.StringStart}{Value}{PdfKeywords.StringEnd}";
	}
}
