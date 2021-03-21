namespace Kwesoft.Pdf
{
	public class PdfInteger : PdfObject<long>
	{
		public override string ToString() => $"{Value}"; 
		
		public static implicit operator PdfInteger(byte value)
		{
			return new PdfInteger { Value = value };
		}

		public static implicit operator PdfInteger(int value)
		{
			return new PdfInteger { Value = value };
		}

		public static implicit operator PdfInteger(long value)
		{
			return new PdfInteger { Value = value };
		}

		public override bool Equals(object obj)
		{
			return obj is PdfInteger other && other.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
