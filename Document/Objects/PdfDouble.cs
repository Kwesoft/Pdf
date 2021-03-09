namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfDouble : PdfObject<double>
	{
		public override string ToString() => $"{Value}";
	}
}
