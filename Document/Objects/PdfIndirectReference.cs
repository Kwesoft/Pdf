using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfIndirectReference : PdfObject
	{
		public int ObjectNumber { get; set; }
		public int GenerationNumber { get; set; }

		public override string ToString()
		{
			return $"{ObjectNumber}{PdfKeywords.Space}{GenerationNumber}{PdfKeywords.Space}{PdfKeywords.Reference}";
		}
	}
}
