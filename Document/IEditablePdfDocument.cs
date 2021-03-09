using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;
using Kwesoft.Pdf.Helpers;
using System;
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
		PdfObject ReadObject(PdfIndirectReference reference);
		PdfKeywordBytes Keywords { get; }
		Encoding Encoding { get; }
		void Replace(int index, int length, byte[] replaceWith);
		void Add(PdfDictionary dictionary, PdfName key, PdfObject value);
		void Remove(PdfDictionary dictionary, PdfName key);
		void Add(PdfArray array, PdfObject value);
		void Remove(PdfArray array, PdfObject value);
		void Remove(PdfArray array, int index);
		void Edit<TPdfObject>(TPdfObject value, Action edit) where TPdfObject : PdfObject;

	}
}
