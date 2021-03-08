using System.Text;

namespace Kwesoft.Pdf.Document.Objects
{
	public abstract class PdfObject
	{
		internal int Offset { get; set; }
		internal int Index => (Parent?.Index ?? 0) + Offset;
		internal int Length { get; set; }
		internal PdfObject Parent { get; set; }

		internal virtual byte[] GetBytes(Encoding encoding) {
			return encoding.GetBytes(ToString());
		}
	}
}
