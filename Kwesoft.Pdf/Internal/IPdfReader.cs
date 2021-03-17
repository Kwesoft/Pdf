
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
