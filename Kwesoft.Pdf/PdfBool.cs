namespace Kwesoft.Pdf
{
	public class PdfBool : PdfObject<bool>
	{
		public override string ToString() => Value ? "true" : "false";

		public static implicit operator PdfBool(bool value)
		{
			return new PdfBool { Value = value };
		}

		public override bool Equals(object obj)
		{
			return obj is PdfBool other && other.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
