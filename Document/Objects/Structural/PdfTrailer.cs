using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects.Structural
{
	class PdfTrailer : PdfObject
	{
		public PdfDictionary TrailerDictionary { get; set; }
		public int CrossReferenceTableOffset { get; set; }

		public override string ToString()
		{
			return $"{PdfKeywords.Trailer}{PdfKeywords.LineBreak}{TrailerDictionary}{PdfKeywords.LineBreak}{PdfKeywords.StartXRef}{PdfKeywords.LineBreak}{CrossReferenceTableOffset}";
		}
	}
}
