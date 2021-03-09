﻿using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;
using Kwesoft.Pdf.Helpers;
using System;
using System.IO;
using System.Text;

namespace Kwesoft.Pdf.Document
{
	public class InMemoryPdf : IEditablePdfDocument, IPdfDocument
	{
		private byte[] _data;
		private readonly IPdfReader _reader;
		public PdfDictionary Root { get; }
		public PdfDictionary Info { get; }
		public PdfHeader Header { get; }

		private readonly PdfCrossReferenceTable _crossReferenceTable;
		PdfCrossReferenceTable IEditablePdfDocument.CrossReferenceTable => _crossReferenceTable;
		private readonly Encoding _encoding;
		Encoding IEditablePdfDocument.Encoding => _encoding;
		private readonly PdfKeywordBytes _keywords;
		PdfKeywordBytes IEditablePdfDocument.Keywords => _keywords;

		private PdfTrailer _trailer { get; }
		PdfTrailer IEditablePdfDocument.Trailer => _trailer;

		private int _length { get; set; }
		int IEditablePdfDocument.Length => _length;

		private IPdfEditor _editor { get; }

		public InMemoryPdf(byte[] data) : this(data, null) 
		{
		}

		internal InMemoryPdf(byte[] data, IPdfReader reader)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			_encoding = Encoding.GetEncoding(1252);
			_keywords = new PdfKeywordBytes(_encoding);

			_data = data;
			_reader = reader ?? new PdfReader(this);
			_length = data.Length;
			Header = _reader.ReadHeader();
			_trailer = _reader.ReadTrailer();
			_crossReferenceTable = _reader.ReadCrossReferenceTable(_trailer);
			
			Root = (PdfDictionary)_reader.ReadObject((PdfIndirectReference)_trailer.TrailerDictionary.Value["Root"]);
			Info = (PdfDictionary)_reader.ReadObject((PdfIndirectReference)_trailer.TrailerDictionary.Value["Info"]);

			_editor = new PdfEditor(this);
		}

		PdfObject IEditablePdfDocument.ReadObject(PdfIndirectReference reference) => _reader.ReadObject(reference);

		byte IEditablePdfDocument.Read(int index) => _data[index];

		byte[] IEditablePdfDocument.Read(int index, int length) {
			var result = new byte[length];
			Buffer.BlockCopy(_data, index, result, 0, length);
			return result;
		}
		
		public void Replace(int index, int length, byte[] replaceWith)
		{
			var newData = new byte[_data.Length + (replaceWith.Length - length)];
			Buffer.BlockCopy(_data, 0, newData, 0, index);
			Buffer.BlockCopy(replaceWith, 0, newData, index, replaceWith.Length);
			Buffer.BlockCopy(_data, index + length, newData, index + replaceWith.Length, _data.Length - (index + length));
			_data = newData;
		}

		public void Save(string filename)
		{
			File.WriteAllBytes(filename, _data);
		}

		public byte[] GetBytes()
		{
			return (byte[])_data.Clone();
		}

		void IEditablePdfDocument.Add(PdfDictionary dictionary, PdfName key, PdfObject value) => _editor.Add(dictionary, key, value);
		void IEditablePdfDocument.Remove(PdfDictionary dictionary, PdfName key) => _editor.Remove(dictionary, key);

		void IEditablePdfDocument.Add(PdfArray array, PdfObject value) => _editor.Add(array, value);
		void IEditablePdfDocument.Remove(PdfArray array, int index) => _editor.Remove(array, index);
		void IEditablePdfDocument.Remove(PdfArray array, PdfObject value) => _editor.Remove(array, value);

		void IEditablePdfDocument.Edit<TPdfObject>(TPdfObject value, Action edit) => _editor.Edit(value, edit);

		PdfIndirectReference IPdfDocument.Add(PdfObject obj) => _editor.Add(obj);
	}
}
