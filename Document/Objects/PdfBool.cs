namespace Kwesoft.Pdf.Document.Objects
{
	class PdfBool : PdfObject
	{
		public bool Value { get; set; }
		public override string ToString() => $"{Value}";
	}
}
