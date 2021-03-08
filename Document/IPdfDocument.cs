using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;

namespace Kwesoft.Pdf.Document
{
	interface IPdfDocument
	{
		PdfDictionary Root { get; }
		PdfDictionary Info { get; }
		PdfHeader Header { get; }
		PdfObject ReadObject(PdfIndirectReference reference);
		IPdfEditor GetEditor();
	}
}
