
namespace Kwesoft.Pdf
{
	internal interface IPdfReader
	{
		decimal ReadVersion();
		PdfTrailer ReadTrailer();
		PdfCrossReferenceTable ReadCrossReferenceTable(PdfTrailer trailer);
		PdfObject ReadObject(PdfIndirectReference reference);
	}
}
