using System;

namespace Kwesoft.Pdf
{
	internal interface IPdfEditor
	{
		PdfIndirectReference Add(PdfObject obj);
		void Add(PdfDictionary dictionary, PdfName key, PdfObject value);
		void Remove(PdfDictionary dictionary, PdfName key);
		void Add(PdfArray array, PdfObject value);
		void Remove(PdfArray array, int index);
		void Remove(PdfArray array, PdfObject value);
		void Edit<TPdfObject>(TPdfObject value, Action edit) where TPdfObject : PdfObject;
	}
}
