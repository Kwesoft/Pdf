namespace Kwesoft.Pdf
{
	public interface IPdfDocument
	{
		PdfDictionary Root { get; }
		PdfDictionary Info { get; }
		decimal Version { get; }
		void Save(string filename);
		byte[] GetBytes();
		PdfIndirectReference Add(PdfObject obj);
		PdfDictionary GetPages();
		long GetPageCount();
	}
}
