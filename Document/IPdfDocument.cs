using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;

namespace Kwesoft.Pdf.Document
{
	public interface IPdfDocument
	{
		PdfDictionary Root { get; }
		PdfDictionary Info { get; }
		PdfHeader Header { get; }
		void Save(string filename);
		byte[] GetBytes();
		PdfIndirectReference Add(PdfObject obj);
	}
}
