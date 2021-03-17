namespace Kwesoft.Pdf
{
	public class PdfBool : PdfObject<bool>
	{
		public override string ToString() => $"{Value}";

		public static implicit operator PdfBool(bool value)
		{
			return new PdfBool { Value = value };
		}
	}
}
