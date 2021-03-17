namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfDoubleReader
	{
		internal static PdfDouble ReadDouble(this IEditablePdfDocument document, int index, int length, PdfObject parent)
		{
			return new PdfDouble
			{
				Value = double.Parse(document.Encoding.GetString(document.Read(index, length))),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = document
			};
		}
	}
}
