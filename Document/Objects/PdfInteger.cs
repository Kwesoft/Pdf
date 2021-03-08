namespace Kwesoft.Pdf.Document.Objects
{
	class PdfInteger : PdfObject
	{
		public int Value { get; set; }
		public override string ToString() => $"{Value}";
	}
}
