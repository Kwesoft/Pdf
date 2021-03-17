namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfIntegerReader
	{
		internal static PdfInteger ReadInteger(this IEditablePdfDocument document, int index, int length, PdfObject parent)
		{
			return new PdfInteger
			{
				Value = int.Parse(document.Encoding.GetString(document.Read(index, length))),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = document
			};
		}
	}
}
