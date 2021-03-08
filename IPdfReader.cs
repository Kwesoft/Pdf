using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;

namespace Kwesoft.Pdf
{
	internal interface IPdfReader
	{
		PdfHeader ReadHeader();
		PdfTrailer ReadTrailer();
		PdfCrossReferenceTable ReadCrossReferenceTable(PdfTrailer trailer);
		PdfObject ReadObject(PdfIndirectReference reference);
	}
}
