using Kwesoft.Pdf.Document.Objects.Structural;
using Kwesoft.Pdf.Helpers;
using System.Text;

namespace Kwesoft.Pdf.Document
{
	internal interface IEditablePdfDocument : IPdfDocument
	{
		PdfCrossReferenceTable CrossReferenceTable { get; }
		PdfTrailer Trailer { get; }
		int Length { get; }
		byte Read(int index);
		byte[] Read(int index, int length);
		PdfKeywordBytes Keywords { get; }
		Encoding Encoding { get; }
		void Replace(int index, int length, byte[] replaceWith);
		void Save(string filename);
	}
}
