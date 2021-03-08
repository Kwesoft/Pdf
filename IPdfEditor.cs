using Kwesoft.Pdf.Document.Objects;
using System;

namespace Kwesoft.Pdf
{
	public interface IPdfEditor
	{
		PdfIndirectReference Add(PdfObject obj);
		void Add(PdfDictionary dictionary, PdfName key, PdfObject value);
		void Add(PdfArray array, PdfString value);
		void Edit<TPdfObject>(TPdfObject value, Action<TPdfObject> edit) where TPdfObject : PdfObject;
		void Save(string filename);
	}
}
