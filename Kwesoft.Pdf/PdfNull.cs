using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf
{
	public class PdfNull : PdfObject
	{
		public override string ToString()
		{
			return PdfKeywords.Null;
		}


		public override bool Equals(object obj)
		{
			return obj is PdfNull other;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
