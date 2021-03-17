using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf
{
	internal class PdfTrailer : PdfObject
	{
		internal PdfDictionary TrailerDictionary { get; set; }
		internal int CrossReferenceTableOffset { get; set; }

		internal PdfIndirectReference Root =>  (PdfIndirectReference)TrailerDictionary.Value["Root"];
		internal PdfIndirectReference Info => (PdfIndirectReference)TrailerDictionary.Value["Info"];

		public override string ToString()
		{
			return $"{PdfKeywords.Trailer}{PdfKeywords.LineBreak}{TrailerDictionary}{PdfKeywords.LineBreak}{PdfKeywords.StartXRef}{PdfKeywords.LineBreak}{CrossReferenceTableOffset}";
		}
	}
}
