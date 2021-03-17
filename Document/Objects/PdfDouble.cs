namespace Kwesoft.Pdf.Document.Objects
{
	public class PdfDouble : PdfObject<double>
	{
		public override string ToString() => $"{Value}";

		public static implicit operator PdfDouble(byte value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfDouble(int value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfDouble(long value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfDouble(float value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfDouble(double value)
		{
			return new PdfDouble { Value = value };
		}

		public static implicit operator PdfDouble(decimal value)
		{
			return new PdfDouble { Value = (double)value };
		}
	}
}
