using Kwesoft.Pdf.Helpers;
using System.Linq;

namespace Kwesoft.Pdf
{
	public class PdfString : PdfObject<string>
	{ 
		public override string ToString() => $"{PdfKeywords.StringStart}{_Sanitise(Value)}{PdfKeywords.StringEnd}";

		public static implicit operator PdfString(string value)
		{
			return new PdfString { Value = value };
		}


		public override bool Equals(object obj)
		{
			return obj is PdfString other && other.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		private string _Sanitise(string input)
		{
			return new string(input.SelectMany(c => {
				return c switch
				{
					'(' => "\\050",
					')' => "\\051",
					'\\' => "\\134",
					_ => $"{c}",
				};
			}).ToArray());
		}
	}
}
