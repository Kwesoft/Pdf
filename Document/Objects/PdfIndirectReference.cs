using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfIndirectReference : PdfObject
	{
		public int ObjectNumber { get; internal set; }
		public int GenerationNumber { get; internal set; }

		public override string ToString()
		{
			return $"{ObjectNumber}{PdfKeywords.Space}{GenerationNumber}{PdfKeywords.Space}{PdfKeywords.Reference}";
		}

		public PdfObject Read()
		{
			if (Document == null) return null;
			return Document.ReadObject(this);
		}
	}
}
