using Kwesoft.Pdf.Document;
using Kwesoft.Pdf.Document.Objects;
using Kwesoft.Pdf.Document.Objects.Structural;
using Kwesoft.Pdf.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kwesoft.Pdf
{
	internal class PdfReader : IPdfReader
	{
		private readonly IEditablePdfDocument _document;
		
		public PdfReader(IEditablePdfDocument document)
		{
			_document = document;
		}

		public PdfHeader ReadHeader()
		{
			var content = _document.Encoding.GetString(_document.Read(0, _Find(2, window => window[0] == 10 && window[1] != 37, _document)));

			switch (content.Split('\n')[0])
			{
				case "%PDF-1.4":
					return new PdfHeader
					{
						Version = 1.4M
					};
			}
			throw new Exception("Invalid _document");
		}

		private PdfName _ReadName(int index, PdfObject parent)
		{
			var end = _Find(1, b => _Equal(b, _document.Keywords.Space) || _Equal(b, _document.Keywords.ArrayEnd) || _Equal(b, _document.Keywords.DictionaryEnd) || _Equal(b, _document.Keywords.LineBreak), _document, index + 1);
			return new PdfName
			{
				Value = _document.Encoding.GetString(_document.Read(index + 1, end - (index + 1))),
				Offset = _GetOffset(index, parent),
				Length = end - index,
				Parent = parent,
				Document = _document
			};
		}

		private PdfArray _ReadArray(int index, PdfObject parent)
		{
			var pos = index + 1;
			var list = new List<PdfObject>();
			var result = new PdfArray(list)
			{
				Offset = _GetOffset(index, parent),
				Parent = parent,
				Document = _document
			};

			while (!_Equal(_document, pos, _document.Keywords.ArrayEnd))
			{
				var obj = _ReadObject(pos, result);
				pos += obj.Length;
				list.Add(obj);
				while (_Equal(_document, pos, _document.Keywords.Space) || _Equal(_document, pos, _document.Keywords.LineBreak))
					pos++;
			}

			result.Length = pos - index;
			return result;
		}

		private PdfString _ReadString(int index, PdfObject parent)
		{
			var length = _Find(_document.Keywords.StringEnd, _document, index) - index;
			return new PdfString
			{
				Offset = _GetOffset(index, parent),
				Length = length + 1,
				Value = _document.Encoding.GetString(_document.Read(index + 1, length - 1)),
				Parent = parent,
				Document = _document
			};
		}

		private PdfDouble _ReadDouble(int index, int length, PdfObject parent)
		{
			return new PdfDouble
			{
				Value = double.Parse(_document.Encoding.GetString(_document.Read(index, length))),
				Offset = _GetOffset(index, parent),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		private PdfInteger _ReadInteger(int index, int length, PdfObject parent)
		{
			return new PdfInteger
			{
				Value = int.Parse(_document.Encoding.GetString(_document.Read(index, length))),
				Offset = _GetOffset(index, parent),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		private bool _IsNumeric(int index)
		{
			return _document.Keywords.Numbers.Contains(_document.Read(index));
		}

		private PdfIndirectReference _ReadIndirectReference(int index, int length, PdfObject parent)
		{
			var text = _document.Encoding.GetString(_document.Read(index, length));
			var indirectReferenceData = text.Split(' ');
			return new PdfIndirectReference
			{
				ObjectNumber = int.Parse(indirectReferenceData[0]),
				GenerationNumber = int.Parse(indirectReferenceData[1]),
				Offset = _GetOffset(index, parent),
				Length = length,
				Parent = parent,
				Document = _document
			};
		}

		public PdfObject ReadObject(PdfIndirectReference reference)
		{
			var index = _Find(_document.Keywords.LineBreak, _document, _document.CrossReferenceTable.ObjectOffsets[reference.ObjectNumber]) + 1;
			var result = _ReadObject(index, null);
			if (_Equal(_document, index + result.Length, _document.Keywords.EndObjLine)) return result;
			var streamStart = _Find(_document.Keywords.StreamStartLine, _document, index + result.Length);
			var end = _Find(_document.Keywords.StreamEndLines, _document, streamStart);
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


		private PdfObject _ReadObject(int index, PdfObject parent)
		{
			if (_Equal(_document, index, _document.Keywords.True)) return new PdfBool { Value = true, Offset = _GetOffset(index, parent), Length = _document.Keywords.True.Length, Parent = parent };
			if (_Equal(_document, index, _document.Keywords.False)) return new PdfBool { Value = false, Offset = _GetOffset(index, parent), Length = _document.Keywords.False.Length, Parent = parent };
			if (_Equal(_document, index, _document.Keywords.Null)) return new PdfNull { Offset = _GetOffset(index, parent), Length = _document.Keywords.Null.Length, Parent = parent };
			if (_Equal(_document, index, _document.Keywords.DictionaryStart)) return _ReadDictionary(index, parent);
			if (_Equal(_document, index, _document.Keywords.StringStart)) return _ReadString(index, parent);
			if (_Equal(_document, index, _document.Keywords.ArrayStart)) return _ReadArray(index, parent);
			if (_Equal(_document, index, _document.Keywords.NameStart)) return _ReadName(index, parent);

			var pos = index;
			var isDecimal = false;

			while (true)
			{
				if (_Equal(_document, pos, _document.Keywords.Decimal))
				{
					isDecimal = true;
				}
				else if (_Equal(_document, pos, _document.Keywords.DictionaryEnd) || _Equal(_document, pos, _document.Keywords.LineBreak) || _Equal(_document, pos, _document.Keywords.ArrayEnd))
				{
					if (isDecimal)
						return _ReadDouble(index, pos - index, parent);
					return _ReadInteger(index, pos - index, parent);
				}
				else if (_Equal(_document, pos, _document.Keywords.Space))
				{
					if (isDecimal)
						return _ReadDouble(index, pos - index, parent);

					var i = 1;
					while (_IsNumeric(pos + i))
						i++;

					if (_Equal(_document, pos + i, _document.Keywords.Space) && _Equal(_document, pos + i + 1, _document.Keywords.Reference))
						return _ReadIndirectReference(index, (pos + i + 2) - index, parent);

					return _ReadInteger(index, pos - index, parent);
				}
				pos++;
			}
		}

		private int _GetOffset(int index, PdfObject parent)
		{
			return index - (parent?.Index ?? 0);
		}

		private PdfDictionary _ReadDictionary(int index, PdfObject parent)
		{
			if (!_Equal(_document, index, _document.Keywords.DictionaryStart)) throw new Exception("Invalid dictionary");
			var pos = index + 2;
			var dictionary = new Dictionary<PdfName, PdfObject>();
			var result = new PdfDictionary(dictionary){ 
				Offset = _GetOffset(index, parent), 
				Parent = parent,
				Document = _document
			};

			while (!_Equal(_document, pos, _document.Keywords.DictionaryEnd))
			{
				if (_Equal(_document, pos, _document.Keywords.NameStart))
				{
					var key = _ReadName(pos, result);
					pos += key.Length + 1;
					var obj = _ReadObject(pos, result);
					dictionary.Add(key, obj);
					pos += obj.Length;
				}
				else
				{
					pos++;
				}
			}
			result.Length = (pos + 2) - index;
			return result;
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
			var eofPos = _FindReverse(_document.Keywords.EOFLine, _document);

			var startxrefPos = _FindReverse(_document.Keywords.StartXRefLine, _document, eofPos);
			var startxrefData = _document.Encoding.GetString(_document.Read(startxrefPos + 11, eofPos - (startxrefPos + 11)));

			var trailerPos = _FindReverse(_document.Keywords.TrailerLine, _document, startxrefPos) + 1;
			var result = new PdfTrailer
			{
				Offset = trailerPos,
				Length = eofPos - trailerPos,
				CrossReferenceTableOffset = int.Parse(startxrefData),
				Document = _document
			};
			result.TrailerDictionary = _ReadDictionary(trailerPos + 8, result);
			return result;
		}

		private static void _Insert(byte[] data, byte newValue)
		{
			for (var i = 1; i < data.Length; i++)
			{
				data[i - 1] = data[i];
			}
			data[data.Length - 1] = newValue;
		}

		private static void _InsertReverse(byte[] data, byte newValue)
		{
			for (var i = data.Length - 1; i > 0; i--)
			{
				data[i] = data[i - 1];
			}
			data[0] = newValue;
		}

		private static bool _Equal(IEditablePdfDocument a, byte[] b) => _Equal(a, 0, b, 0);
		private static bool _Equal(IEditablePdfDocument a, int posA, byte[] b) => _Equal(a, posA, b, 0);
		private static bool _Equal(IEditablePdfDocument a, int posA, byte[] b, int posB) => _Equal(a.Read(posA, b.Length - posB), 0, b, posB);

		private static bool _Equal(byte[] a, byte[] b) => _Equal(a, 0, b, 0);
		private static bool _Equal(byte[] a, int posA, byte[] b) => _Equal(a, posA, b, 0);
		private static bool _Equal(byte[] a, int posA, byte[] b, int posB)
		{
			var lengthA = a.Length - posA;
			var lengthB = b.Length - posB;
			var length = lengthA < lengthB ? lengthA : lengthB;
			for (var i = 0; i < length; i++)
			{
				if (a[posA + i] != b[posB + i]) return false;
			}
			return true;
		}

		private static int _Find(byte[] lookFor, IEditablePdfDocument document, int start) => _Find(lookFor.Length, w => _Equal(w, lookFor), document, start);
		private static int _Find(byte[] lookFor, IEditablePdfDocument document) => _Find(lookFor, document, 0);
		private static int _Find(int windowSize, Func<byte[], bool> matches, IEditablePdfDocument document, int start = 0)
		{
			var end = document.Length;

			var pos = start;
			var window = new byte[windowSize];
			while (pos != end)
			{
				_Insert(window, document.Read(pos));
				if (matches(window)) return pos;
				pos++;
			}
			return -1;
		}
		private static int _FindReverse(int windowSize, Func<byte[], bool> matches, IEditablePdfDocument document) => _FindReverse(windowSize, matches, document, document.Length - 1);
		private static int _FindReverse(byte[] lookFor, IEditablePdfDocument document, int start) => _FindReverse(lookFor.Length, w => _Equal(w, lookFor), document, start);
		private static int _FindReverse(byte[] lookFor, IEditablePdfDocument document) => _FindReverse(lookFor, document, document.Length - 1);
		private static int _FindReverse(int windowSize, Func<byte[], bool> matches, IEditablePdfDocument document, int start)
		{
			var end = 0;

			var pos = start;
			var window = new byte[windowSize];
			while (pos != end)
			{
				_InsertReverse(window, document.Read(pos));
				if (matches(window)) return pos;
				pos--;
			}
			return -1;
		}
	}
}
