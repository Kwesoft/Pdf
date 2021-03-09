namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfInteger : PdfObject<int>
	{
		public override string ToString() => $"{Value}";
	}
}
