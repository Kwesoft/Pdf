namespace Kwesoft.Pdf.Helpers
{
	internal static class PdfIndirectReferenceReader
	{
		internal static PdfIndirectReference ReadIndirectReference(this IEditablePdfDocument document, int index, int length, PdfObject parent)
		{
			var text = document.Encoding.GetString(document.Read(index, length));
			var indirectReferenceData = text.Split(' ');
			return new PdfIndirectReference
			{
				ObjectNumber = int.Parse(indirectReferenceData[0]),
				GenerationNumber = int.Parse(indirectReferenceData[1]),
				Offset = index - (parent?.Index ?? 0),
				Length = length,
				Parent = parent,
				Document = document
			};
		}
	}
}
