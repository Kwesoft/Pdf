using Kwesoft.Pdf.Helpers;

namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfName : PdfObject<string>
	{
		public static implicit operator PdfName(string s)
		{
			return new PdfName { Value = s };
		}

		public override bool Equals(object obj)
		{
			return obj is PdfName other && other.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString() {
			return $"{PdfKeywords.NameStart}{Value}";
		}
	}
}
