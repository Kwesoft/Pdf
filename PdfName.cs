using Kwesoft.Pdf.Helpers;
using System.Linq;

namespace Kwesoft.Pdf
{
	public class PdfName : PdfObject<string>
	{
		public static implicit operator PdfName(string value)
		{
			return new PdfName { Value = value };
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
			return $"{PdfKeywords.NameStart}{_Sanitise(Value)}";
		}

		private string _Sanitise(string input)
		{
			return new string(input.SelectMany(c => {
				if (c < 0x21 || c > 0x7e || c == '%' || c == '(' || c == ')' || c == '<' || c == '>' || c == '[' || c == ']' || c == '{' || c == '}' || c == '/' || c == '#')
					return $"#{((int)c).ToString("X").PadLeft(2, '0')}";
				return $"{c}";
			}).ToArray());
		}
	}
}
