namespace Kwesoft.Pdf
{
	public class PdfBool : PdfObject<bool>
	{
		public override string ToString() => Value ? "true" : "false";

		public static implicit operator PdfBool(bool value)
		{
			return new PdfBool { Value = value };
		}
	}
}
