using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfNull : PdfObject
	{
		public override string ToString()
		{
			return PdfKeywords.Null;
		}
	}
}
