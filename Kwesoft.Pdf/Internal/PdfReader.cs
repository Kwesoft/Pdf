using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kwesoft.Pdf
{
	internal class PdfReader : IPdfReader
	{
		private readonly IEditablePdfDocument _document;

		public PdfReader(IEditablePdfDocument document)
		{
			_document = document;
		}

		public decimal ReadVersion()
		{
			var content = _document.Encoding.GetString(_document.Read(0, _document.Find(2, window => window[0] == 10 && window[1] != 37)));

			switch (content.Split('\n')[0])
			{
				case "%PDF-1.4":
					return 1.4M;
			}
			throw new Exception("Invalid document");
		}

		public PdfObject ReadObject(PdfIndirectReference reference)
		{
			var index = _document.Find(_document.Keywords.LineBreak, _document.CrossReferenceTable.ObjectOffsets[reference.ObjectNumber]) + 1;
			var result = _document.ReadObject(index, null);
			if (_document.ByteEquals(index + result.Length, _document.Keywords.EndObjLine)) return result;
			var streamStart = _document.Find(_document.Keywords.StreamStartLine, index + result.Length);
			var end = _document.Find(_document.Keywords.StreamEndLines, streamStart);
			var stream = new PdfStream
			{
				Properties = (PdfDictionary)result,
				Offset = index,
				Length = end - index,
				Data = _document.Read(streamStart, end - _document.Keywords.StreamEndLines.Length - streamStart),
				Document = _document
			};
			result.Parent = stream;
			return stream;
		}


		public PdfCrossReferenceTable ReadCrossReferenceTable(PdfTrailer trailer)
		{
			var pos = trailer.CrossReferenceTableOffset;
			var length = trailer.Index - pos;
			var crossReferenceData = _document.Encoding.GetString(_document.Read(pos, length - 1)).Split(new[] { PdfKeywords.LineBreak }, StringSplitOptions.RemoveEmptyEntries);

			var crossReferenceTableSummary = crossReferenceData[1].Split(new[] { PdfKeywords.Space }, StringSplitOptions.RemoveEmptyEntries);
			var firstObjectNumber = int.Parse(crossReferenceTableSummary[0]);
			var objectCount = int.Parse(crossReferenceTableSummary[1]);

			var result = new PdfCrossReferenceTable
			{
				ObjectOffsets = new Dictionary<int, int>(),
				Offset = pos,
				Length = length,
				FirstObjectNumber = firstObjectNumber,
				ObjectCount = objectCount,
				Document = _document
			};

			for (var i = 2; i < crossReferenceData.Length; i++)
			{
				result.ObjectOffsets.Add(firstObjectNumber + i - 2, int.Parse(crossReferenceData[i].Split(new[] { PdfKeywords.Space }, StringSplitOptions.RemoveEmptyEntries)[0]));
			}

			return result;
		}

		public PdfTrailer ReadTrailer()
		{
			var eofPos = _document.FindReverse(_document.Keywords.EOFLine);

			var startxrefPos = _document.FindReverse(_document.Keywords.StartXRefLine, eofPos);
			var startxrefData = _document.Encoding.GetString(_document.Read(startxrefPos + 11, eofPos - (startxrefPos + 11)));

			var trailerPos = _document.FindReverse(_document.Keywords.TrailerLine, startxrefPos) + 1;
			var result = new PdfTrailer
			{
				Offset = trailerPos,
				Length = eofPos - trailerPos,
				CrossReferenceTableOffset = int.Parse(startxrefData),
				Document = _document
			};
			result.TrailerDictionary = _document.ReadDictionary(trailerPos + 8, result);
			return result;
		}
	}
}
