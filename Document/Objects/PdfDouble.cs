namespace Kwesoft.Pdf.Document.Objects
{
	class PdfDouble : PdfObject
	{
		public double Value { get; set; }
		public override string ToString() => $"{Value}";
	}
}
