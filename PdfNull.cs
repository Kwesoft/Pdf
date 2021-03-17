using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf
{
	public class PdfNull : PdfObject
	{
		public override string ToString()
		{
			return PdfKeywords.Null;
		}
	}
}
