namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfBool : PdfObject<bool>
	{
		public override string ToString() => $"{Value}";
	}
}
